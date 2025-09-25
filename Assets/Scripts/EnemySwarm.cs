using System.Collections.Generic;
using UnityEngine;

public class EnemySwarm : MonoBehaviour
{
    [Header("Layout Settings")]
    public GameObject enemyPrefab;
    public int rows = 5;
    public int columns = 11;
    public float horizontalSpacing = 1.2f;
    public float verticalSpacing = 1.2f;
    public Vector2 startOffset = new(-6f, 3.5f);
    
    [Header("Movement Settings")]
    public float speed = 2f;
    public float stepDown = 0.4f;
    public float moveBoundaryX = 7.5f;
    
    [Header("Shooting Settings")]
    public ObjectPool enemyBulletPool;
    public float shootIntervalMin = 1.5f;
    public float shootIntervalMax = 3.5f;
    public float enemyBulletSpeed = 8f;
    
    public List<Enemy> _enemies = new();
    private int _direction = 1; // 1 = right, -1 = left
    private float _shootTimer;
    
    public System.Action OnSwarmReachedBottom; // Evento para notificar cuando el enjambre llega al fondo
    public System.Action<int> OnEnemyKilled; // Evento para notificar cuando un enemigo es eliminado, con el valor de puntaje

    void Start()
    {
        SpawnGrid();
        ResetShotTimer();
    }
    
    void Update()
    {
        if (_enemies.Count == 0) return;
        
        // Movimiento
        transform.position += Vector3.right * (_direction * speed * Time.deltaTime);
        
        // Verificar los limiites con el enemigo más a la derecha e izquierda
        float minX = float.MaxValue;
        float maxX = float.MinValue;
        float minY = float.MaxValue;
        foreach(var enemy in _enemies)
        {
            if (enemy == null) continue;
            
            var x = enemy.transform.position.x;
            var y = enemy.transform.position.y;
            if (x < minX) minX = x;
            if (x > maxX) maxX = x;
            if (y < minY) minY = y;
        }

        if (maxX >= moveBoundaryX && _direction == 1)
        {
            _direction = -1;
            transform.position += Vector3.down * stepDown;
        }
        else if (minX <= -moveBoundaryX && _direction == -1)
        {
            _direction = 1;
            transform.position += Vector3.down * stepDown;
        }
        
        // Verificar si el enjambre ha llegado al fondo
        if (minY <= -4.5f)
        {
            OnSwarmReachedBottom?.Invoke();
        }
        
        // Disparo
        _shootTimer -= Time.deltaTime;
        if (_shootTimer <= 0f)
        {
            ShootFromRandomBottomEnemy();
            ResetShotTimer();
        }
        
        float t = Mathf.InverseLerp(columns * rows, 1, _enemies.Count);
        speed = Mathf.Lerp(1f, 5f, t);
    }

    private void SpawnGrid()
    {
        _enemies.Clear();
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < columns; c++)
            {
                Vector3 local = new (startOffset.x + c * horizontalSpacing, startOffset.y + r * verticalSpacing, 0f);
                var enemyGO = Instantiate(enemyPrefab, transform.position + local, Quaternion.identity, transform);
                var enemy = enemyGO.GetComponent<Enemy>();
                if(enemy is not null)
                {
                    enemy.OnKilled += OnEnemyKilledHandler; 
                    _enemies.Add(enemy);
                }
            }
        }
    }
    
    private void OnEnemyKilledHandler(Enemy enemy)
    {
        OnEnemyKilled?.Invoke(enemy.scoreValue);
        _enemies.Remove(enemy);
    }

    private void ResetShotTimer()
    {
        _shootTimer = Random.Range(shootIntervalMin, shootIntervalMax);
    }
    
}
