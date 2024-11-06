using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSController : MonoBehaviour
{
    public static FPSController instance; // Instancia estática para acceder desde otros scripts

    public float walkSpeed = 6f;
    public float sprintSpeed = 12f;
    public float jumpHeight = 2f;
    public float gravity = -9.81f;
    public Transform playerCamera;
    public float lookSensitivity = 2f;
    public float maxLookX = 60f;
    public float minLookX = -60f;

    private CharacterController characterController;
    private Vector3 moveDirection;
    private Vector3 velocity;
    private bool isGrounded;
    private float rotX = 0f;

    void Awake()
    {
        // Asegurar que solo haya una instancia de FPSController
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        Move();
        Look();
    }

    void Move()
    {
        isGrounded = characterController.isGrounded;

        // Movimiento basado en input
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        // Sprinting
        float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed;

        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        moveDirection = move * currentSpeed;

        // Salto
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        characterController.Move(moveDirection * Time.deltaTime);

        // Aplicar gravedad
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);
    }

    void Look()
    {
        float mouseX = Input.GetAxis("Mouse X") * lookSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * lookSensitivity;

        // Rotación vertical (eje Y de la cámara)
        rotX -= mouseY;
        rotX = Mathf.Clamp(rotX, minLookX, maxLookX);

        // Aplicar la rotación vertical a la cámara
        playerCamera.localRotation = Quaternion.Euler(rotX, 0f, 0f);

        // Rotación horizontal (eje X del jugador)
        transform.Rotate(Vector3.up * mouseX);
    }
}
