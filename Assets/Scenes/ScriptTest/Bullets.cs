using UnityEngine;

public class Bullets : MonoBehaviour
{
    public float speed = 20f; // Velocidad del rayo
    public int damage = 30; // Daño infligido por la bala
    public float range = 100f; // Rango máximo del disparo
    public GameObject impactEffect; // Prefab para el efecto de impacto

    void Start()
    {
        // Destruye la bala después de 5 segundos si no impacta nada
        Destroy(gameObject, 5f);
    }

    void Update()
    {
        // Mover la bala hacia adelante usando un rayo
        Shoot();
    }

    void Shoot()
    {
        // Definimos el origen y dirección del disparo
        Vector3 origin = transform.position; // La posición del objeto bala
        Vector3 direction = transform.forward; // La dirección hacia donde apunta

        RaycastHit hit;

        // Lanza un rayo desde el origen en la dirección indicada
        if (Physics.Raycast(origin, direction, out hit, range))
        {
            // Primero verificamos si golpea un zombi
            ZombieController zombie = hit.collider.GetComponent<ZombieController>();
            if (zombie != null)
            {
                zombie.TakeDamage(damage); // Inflige daño al zombi
            }

            // Generar efecto de impacto si se ha definido uno
            if (impactEffect != null)
            {
                Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
            }

            // Destruir la bala después del impacto
            Destroy(gameObject);
        }

        // Mueve la bala hacia adelante
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    void OnDestroy()
    {
        // Aquí podrías añadir lógica para manejar el efecto de destrucción, como un efecto visual de la bala
    }
}
