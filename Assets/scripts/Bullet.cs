using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage = 30;

    void OnCollisionEnter(Collision collision)
    {
        ZombieController zombie = collision.collider.GetComponent<ZombieController>();
        if (zombie != null)
        {
            zombie.TakeDamage(damage);
            Destroy(gameObject); // Destruir la bala después de impactar
        }
    }
}
