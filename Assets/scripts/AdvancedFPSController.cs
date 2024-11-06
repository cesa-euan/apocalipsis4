using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class AdvancedFPSController : MonoBehaviour
{

    [Header("Movement Settings")]
    public float walkSpeed = 6f;
    public float sprintSpeed = 12f;
    public float crouchSpeed = 3f;
    public float gravity = -9.81f;
    public float jumpHeight = 2f;
    public static AdvancedFPSController instance;
    public GameObject player;
    [Header("Camera Settings")]
    public Transform playerCamera;
    public float lookSensitivity = 2f;
    public float maxLookX = 60f;
    public float minLookX = -60f;
    private float rotX = 0f;

    [Header("Crouch Settings")]
    public float crouchHeight = 1f;
    public float standingHeight = 2f;
    public float crouchTransitionSpeed = 5f;
    private float originalCenterY;

    private CharacterController characterController;
    private Vector3 moveDirection;
    private float currentSpeed;
    private bool isSprinting;
    private bool isCrouching;

    private Vector3 velocity;
    private bool isGrounded;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;

        // Guardar el valor original del centro del CharacterController
        originalCenterY = characterController.center.y;
    }

    void Update()
    {
        HandleMovement();
        HandleCamera();
        HandleCrouch();
        ApplyGravity();
    }


    void HandleMovement()
    {
        isGrounded = characterController.isGrounded;

        // Movimiento basado en input
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        // Sprinting
        if (Input.GetKey(KeyCode.LeftShift) && !isCrouching)
        {
            isSprinting = true;
            currentSpeed = sprintSpeed;
        }
        else
        {
            isSprinting = false;
            currentSpeed = isCrouching ? crouchSpeed : walkSpeed;
        }

        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        moveDirection = move * currentSpeed;

        // Saltar
        if (Input.GetButtonDown("Jump") && isGrounded && !isCrouching)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        characterController.Move(moveDirection * Time.deltaTime);
    }

    void HandleCamera()
    {
        // Movimiento del mouse
        float mouseX = Input.GetAxis("Mouse X") * lookSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * lookSensitivity;

        // Rotación vertical (eje Y de la cámara)
        rotX -= mouseY;
        rotX = Mathf.Clamp(rotX, minLookX, maxLookX); // Limitar el ángulo de rotación vertical

        // Aplicar la rotación vertical a la cámara
        playerCamera.localRotation = Quaternion.Euler(rotX, 0f, 0f);

        // Rotación horizontal (eje X del jugador)
        transform.Rotate(Vector3.up * mouseX);
    }

    void HandleCrouch()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            isCrouching = !isCrouching;
        }

        // Altura objetivo del CharacterController
        float targetHeight = isCrouching ? crouchHeight : standingHeight;

        // Guardamos la altura actual antes del cambio
        float previousHeight = characterController.height;

        // Aplicamos la interpolación de altura
        characterController.height = Mathf.Lerp(characterController.height, targetHeight, Time.deltaTime * crouchTransitionSpeed);

        // Compensamos el cambio en la posición vertical para evitar temblores
        float heightDifference = previousHeight - characterController.height;
        characterController.center = new Vector3(characterController.center.x, originalCenterY - heightDifference / 2, characterController.center.z);
    }

    void ApplyGravity()
    {
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;  // Pequeño valor negativo para mantenerse en el suelo
        }

        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);
    }
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }


    }
}
