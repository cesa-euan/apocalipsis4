using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public enum FireMode { SemiAuto, FullAuto }
    public FireMode fireMode = FireMode.FullAuto;

    public Transform cameraTransform;
    public float bulletSpeed = 20f;
    public float fireRate = 0.1f; // Tasa de fuego rápida para automático
    public Animator animator;

    public AudioSource audioSource;
    public AudioClip shootSound;
    public AudioClip reloadSound;
    public AudioClip switchWeaponSound;
    public AudioClip[] walkSounds; // Arreglo para variantes de sonido de caminar
    public AudioClip[] sprintSounds; // Arreglo para variantes de sonido de correr

    public Light muzzleFlashLight; // Luz de destello de disparo
    public CameraController cameraController; // Referencia al script de la cámara

    public int maxAmmo = 30;
    public int reserveAmmo = 90; // Balas en reserva
    [SerializeField] private int currentAmmo; // Visible en el Inspector
    private float nextTimeToFire = 0f;
    private float lightDuration = 0.05f; // Duración del destello de luz
    private float walkSoundCooldown = 0.5f; // Tiempo entre sonidos de pasos al caminar
    private float sprintSoundCooldown = 0.3f; // Tiempo entre sonidos de pasos al correr
    private float nextWalkSoundTime = 0f;
    private float nextSprintSoundTime = 0f;

    void Start()
    {
        currentAmmo = maxAmmo;
        muzzleFlashLight.enabled = false; // Asegúrate de que la luz esté apagada al inicio
    }

    void Update()
    {
        HandleShooting();
        HandleWeaponAnimation();
        PlayMovementSounds();
    }

    void HandleShooting()
    {
        if ((fireMode == FireMode.FullAuto && Input.GetButton("Fire1")) ||
            (fireMode == FireMode.SemiAuto && Input.GetButtonDown("Fire1")))
        {
            if (Time.time >= nextTimeToFire && currentAmmo > 0)
            {
                nextTimeToFire = Time.time + 1f / fireRate;
                Shoot();
            }
        }
    }

    void Shoot()
    {
        currentAmmo--; // Reducir el conteo de balas

        // Realizar el raycast
        Vector3 origin = cameraTransform.position; // Posición de la cámara
        Vector3 direction = cameraTransform.forward; // Dirección del disparo
        RaycastHit hit;

        if (Physics.Raycast(origin, direction, out hit, Mathf.Infinity))
        {
            // Comprobar si golpea un zombi
            ZombieController zombie = hit.collider.GetComponent<ZombieController>();
            if (zombie != null)
            {
                zombie.TakeDamage(30); // Inflige daño al zombi
            }

            // Aquí podrías instanciar un efecto de impacto
            // Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
        }

        animator.Play("Shoot", 0, 0f); // Reiniciar la animación de disparo
        audioSource.PlayOneShot(shootSound); // Reproducir el sonido de disparo

        StartCoroutine(FlashMuzzleLight()); // Activar la luz de destello
        cameraController.ShakeCamera(); // Sacudir la cámara al disparar
    }

    IEnumerator FlashMuzzleLight()
    {
        muzzleFlashLight.enabled = true;
        yield return new WaitForSeconds(lightDuration);
        muzzleFlashLight.enabled = false;
    }

    void HandleWeaponAnimation()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        if (Input.GetKey(KeyCode.LeftShift))
        {
            animator.SetBool("isSprinting", true);
            if (Time.time >= nextSprintSoundTime && (moveX != 0 || moveZ != 0))
            {
                PlayRandomSprintSound();
                nextSprintSoundTime = Time.time + sprintSoundCooldown;
            }
        }
        else
        {
            animator.SetBool("isSprinting", false);
            if (moveX != 0 || moveZ != 0)
            {
                animator.SetBool("isWalking", true);
                if (Time.time >= nextWalkSoundTime)
                {
                    PlayRandomWalkSound();
                    nextWalkSoundTime = Time.time + walkSoundCooldown;
                }
            }
            else
            {
                animator.SetBool("isWalking", false);
            }
        }

        if (Input.GetKeyDown(KeyCode.R) && currentAmmo < maxAmmo && reserveAmmo > 0)
        {
            Reload();
        }
    }

    void Reload()
    {
        animator.SetTrigger("Reload");
        audioSource.PlayOneShot(reloadSound);

        int bulletsNeeded = maxAmmo - currentAmmo;
        int bulletsToReload = Mathf.Min(bulletsNeeded, reserveAmmo);

        currentAmmo += bulletsToReload;
        reserveAmmo -= bulletsToReload;
    }

    void PlayMovementSounds()
    {
        if ((Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0) && !audioSource.isPlaying)
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                PlayRandomSprintSound();
            }
            else
            {
                PlayRandomWalkSound();
            }
        }
    }

    void PlayRandomWalkSound()
    {
        if (walkSounds.Length > 0)
        {
            int index = Random.Range(0, walkSounds.Length);
            audioSource.PlayOneShot(walkSounds[index]);
        }
    }

    void PlayRandomSprintSound()
    {
        if (sprintSounds.Length > 0)
        {
            int index = Random.Range(0, sprintSounds.Length);
            audioSource.PlayOneShot(sprintSounds[index]);
        }
    }

    public void SwitchWeapon()
    {
        animator.SetTrigger("SwitchWeapon");
        audioSource.PlayOneShot(switchWeaponSound);
    }

    public void AddAmmo(int amount)
    {
        reserveAmmo += amount;
    }
}
