using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Vector2 _dir;
    private float _speed;
    private GameObject _owner;
    private ObjectPool _returnPool;

    public void Init(Vector2 dir, float speed, GameObject owner, ObjectPool pool)
    {
        _dir = dir.normalized;
        _speed = speed;
        _owner = owner;
        _returnPool = pool;
    }

    private void Update()
    {
        transform.Translate(_dir * _speed * Time.deltaTime);

        // Despawn si sale de pantalla
        if (Mathf.Abs(transform.position.y) > 12f)
        {
            _returnPool.Return(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject == _owner)
        {
            Debug.Log("nothing hit");
            return; // ignora al que disparó
        }

        // Daño a enemigos, jugador o escudos
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            var enemy = other.GetComponentInParent<Enemy>();
            Debug.Log("hit enemy");
            enemy.Kill();
            _returnPool.Return(gameObject);
        }
        else if (other.gameObject.TryGetComponent(out PlayerController player))
        {
            Debug.Log("hit player");
            player.Damage();
            _returnPool.Return(gameObject);
        }
        else if (other.gameObject.layer == LayerMask.NameToLayer("Shield"))
        {
            Debug.Log("hit shield");
            // Escudo se daña/destroza
            var shield = other.GetComponent<ShieldChunk>();
            if (shield != null) shield.Damage(1);
            _returnPool.Return(gameObject);
        }
    }
}