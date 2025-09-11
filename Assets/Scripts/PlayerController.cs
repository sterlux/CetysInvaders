using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float speed = 14f;
    public float clampX = 11f;
    
    private PlayerControls _controls;
    private Vector2 _moveX;
    
    public ObjectPool bulletPool;
    public Transform bulletSpawn;
    public float fireCooldown = 0.25f;
    private float _cooldownTimer;

    void Awake()
    {
        _controls = new PlayerControls();
    }

    private void OnEnable()
    {
        _controls.Enable();
        _controls.Gameplay.Move.performed += OnMove;
        _controls.Gameplay.Move.canceled += OnMove;
        _controls.Gameplay.Fire.performed += OnFire;
    }

    private void OnFire(InputAction.CallbackContext context)
    {
        TryShoot();
    }
    private void OnMove(InputAction.CallbackContext context)
    {
        _moveX = context.ReadValue<Vector2>();
    }

    // Update is called once per frame
    void Update()
    {
        _cooldownTimer -= Time.deltaTime;

        var pos = transform.position;
        pos.x += _moveX.x * speed * Time.deltaTime;
        pos.x = Mathf.Clamp(pos.x, -clampX, clampX);
        transform.position = pos;
    }
    
    private void TryShoot()
    {
        // if (_cooldownTimer > 0f || bulletPool == null || bulletSpawn == null) return;
        // _cooldownTimer = fireCooldown;

        var bullet = bulletPool.Get(bulletSpawn.position, Quaternion.identity);
        var proj = bullet.GetComponent<Projectile>();
        proj.Init(Vector2.up, 12f, gameObject, bulletPool);
    }
}
