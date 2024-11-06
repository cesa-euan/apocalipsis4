using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdvancedShootingController : MonoBehaviour
{
    [Header("Shooting Settings")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float projectileSpeed = 20f;
    public float fireRate = 0.2f; // Tasa de disparo para arma autom�tica
    public int maxAmmo = 30; // Munici�n m�xima
    public int currentAmmo; // Munici�n actual

    [Header("Animation Settings")]
    public Animator animator;

    [Header("Audio Settings")]
    public AudioSource audioSource; // Componente AudioSource
    public AudioClip shootSound; // Sonido de disparo
    public AudioClip reloadSound; // Sonido de recarga

    [Header("Effects Settings")]
    public GameObject flashEffect; // Prefab del destello (efecto visual de disparo)
    public Light muzzleLight; // Luz que simula el fogonazo
    public GameObject bulletHitEffect; // Prefab de part�culas que se activa al disparar
    public float lightDuration = 0.05f; // Duraci�n del fogonazo (luz)

    private float nextFireTime = 0f;
    private bool isReloading = false; // Bandera para controlar si est� recargando

    void Start()
    {
        currentAmmo = maxAmmo; // Inicia la munici�n actual
        muzzleLight.enabled = false; // Aseg�rate de que la luz est� apagada al inicio
    }

    void Update()
    {
        // Si el jugador mantiene presionado el bot�n de disparo
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

        // Si el jugador presiona el bot�n de recarga y no est� recargando
        if ((Input.GetKeyDown(KeyCode.R) && !isReloading) || currentAmmo <= 0)
        {
            // Agregar condici�n para verificar si el cargador tiene menos de 30 balas
            if (currentAmmo < maxAmmo)
            {
                StartCoroutine(Reload());
            }
        }
    }

    void Shoot()
    {
        // Verificar si la munici�n es 0 para no disparar
        if (currentAmmo <= 0)
        {
            Debug.Log("Sin munici�n");
            return;
        }

        // Verificar si est� recargando antes de disparar
        if (isReloading)
        {
            Debug.Log("No se puede disparar, recargando..."); // Mensaje de depuraci�n
            return; // Salir si est� recargando
        }

        nextFireTime = Time.time + fireRate; // Actualiza el tiempo para el siguiente disparo

        // Instancia el proyectil
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        rb.velocity = firePoint.forward * projectileSpeed;

        // Disminuir la munici�n
        currentAmmo--;

        // Reproduce la animaci�n de disparo
        animator.SetTrigger("Shoot");

        // Reproduce el sonido de disparo
        audioSource.PlayOneShot(shootSound);

        // Activar el efecto de part�culas al disparar
        if (bulletHitEffect != null)
        {
            Instantiate(bulletHitEffect, firePoint.position, firePoint.rotation);
        }
    }

    IEnumerator Reload()
    {
        if (isReloading) yield break; // Evitar que se inicie otra recarga si ya est� recargando.

        isReloading = true; // Marcar el estado de recarga como verdadero

        // Reproduce la animaci�n de recarga
        animator.SetTrigger("Reload");

        // Reproduce el sonido de recarga solo una vez
        audioSource.PlayOneShot(reloadSound);

        yield return new WaitForSeconds(1f); // Duraci�n de la recarga (animaci�n)

        // Restaurar la munici�n al m�ximo solo despu�s de que la animaci�n se complete
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
            Destroy(flash, 0.1f); // Destruir el destello despu�s de 0.1 segundos
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
        yield return new WaitForSeconds(lightDuration); // Espera la duraci�n de la luz
        muzzleLight.enabled = false; // Desactiva la luz
    }
}
