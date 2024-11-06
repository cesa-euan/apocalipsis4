using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdvancedShootingController : MonoBehaviour
{
    [Header("Shooting Settings")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float projectileSpeed = 20f;
    public float fireRate = 0.2f; // Tasa de disparo para arma automática
    public int maxAmmo = 30; // Munición máxima
    public int currentAmmo; // Munición actual

    [Header("Animation Settings")]
    public Animator animator;

    [Header("Audio Settings")]
    public AudioSource audioSource; // Componente AudioSource
    public AudioClip shootSound; // Sonido de disparo
    public AudioClip reloadSound; // Sonido de recarga

    [Header("Effects Settings")]
    public GameObject flashEffect; // Prefab del destello (efecto visual de disparo)
    public Light muzzleLight; // Luz que simula el fogonazo
    public GameObject bulletHitEffect; // Prefab de partículas que se activa al disparar
    public float lightDuration = 0.05f; // Duración del fogonazo (luz)

    private float nextFireTime = 0f;
    private bool isReloading = false; // Bandera para controlar si está recargando

    void Start()
    {
        currentAmmo = maxAmmo; // Inicia la munición actual
        muzzleLight.enabled = false; // Asegúrate de que la luz esté apagada al inicio
    }

    void Update()
    {
        // Si el jugador mantiene presionado el botón de disparo
        if (Input.GetButton("Fire1") && currentAmmo > 0 && !isReloading)
        {
            if (Time.time >= nextFireTime)
            {
                Shoot(); // Dispara si el tiempo lo permite
                ActivateFlashAndLight(); // Activa el destello y la luz
            }
        }
        else
        {
            // Apagar el destello y la luz cuando no se dispara
            muzzleLight.enabled = false;
        }

        // Si el jugador presiona el botón de recarga y no está recargando
        if ((Input.GetKeyDown(KeyCode.R) && !isReloading) || currentAmmo <= 0)
        {
            // Agregar condición para verificar si el cargador tiene menos de 30 balas
            if (currentAmmo < maxAmmo)
            {
                StartCoroutine(Reload());
            }
        }
    }

    void Shoot()
    {
        // Verificar si la munición es 0 para no disparar
        if (currentAmmo <= 0)
        {
            Debug.Log("Sin munición");
            return;
        }

        // Verificar si está recargando antes de disparar
        if (isReloading)
        {
            Debug.Log("No se puede disparar, recargando..."); // Mensaje de depuración
            return; // Salir si está recargando
        }

        nextFireTime = Time.time + fireRate; // Actualiza el tiempo para el siguiente disparo

        // Instancia el proyectil
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        rb.velocity = firePoint.forward * projectileSpeed;

        // Disminuir la munición
        currentAmmo--;

        // Reproduce la animación de disparo
        animator.SetTrigger("Shoot");

        // Reproduce el sonido de disparo
        audioSource.PlayOneShot(shootSound);

        // Activar el efecto de partículas al disparar
        if (bulletHitEffect != null)
        {
            Instantiate(bulletHitEffect, firePoint.position, firePoint.rotation);
        }
    }

    IEnumerator Reload()
    {
        if (isReloading) yield break; // Evitar que se inicie otra recarga si ya está recargando.

        isReloading = true; // Marcar el estado de recarga como verdadero

        // Reproduce la animación de recarga
        animator.SetTrigger("Reload");

        // Reproduce el sonido de recarga solo una vez
        audioSource.PlayOneShot(reloadSound);

        yield return new WaitForSeconds(1f); // Duración de la recarga (animación)

        // Restaurar la munición al máximo solo después de que la animación se complete
        currentAmmo = maxAmmo;

        // Salir del estado de recarga
        isReloading = false;
    }

    void ActivateFlashAndLight()
    {
        // Instanciar el efecto de destello
        if (flashEffect != null)
        {
            GameObject flash = Instantiate(flashEffect, firePoint.position, firePoint.rotation);
            Destroy(flash, 0.1f); // Destruir el destello después de 0.1 segundos
        }

        // Activar la luz
        if (muzzleLight != null)
        {
            muzzleLight.enabled = true;
            StartCoroutine(DeactivateLight());
        }
    }

    IEnumerator DeactivateLight()
    {
        yield return new WaitForSeconds(lightDuration); // Espera la duración de la luz
        muzzleLight.enabled = false; // Desactiva la luz
    }
}
