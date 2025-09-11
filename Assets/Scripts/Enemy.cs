using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int scoreValue = 100;
    public System.Action<Enemy> OnKilled; // Evento para notificar cuando el enemigo es eliminado
    
    public void Kill()
    {
        // Aquí puedes agregar efectos de muerte, sonidos, etc.
        OnKilled?.Invoke(this); // Notificar a los suscriptores que este enemigo ha sido eliminado
        Destroy(gameObject); // Destruir el objeto enemigo
    }
}
