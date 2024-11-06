using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float turnSpeed = 720f;
    public Camera playerCamera;
    public float lookSensitivity = 2f;
    public float maxLookX = 60f;
    public float minLookX = -60f;

    private CharacterController characterController;
    private Vector3 moveDirection;
    private float rotX = 0f;

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
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        moveDirection = move * moveSpeed;

        characterController.Move(moveDirection * Time.deltaTime);
    }

    void Look()
    {
        float mouseX = Input.GetAxis("Mouse X") * lookSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * lookSensitivity;

        // Rotaci�n vertical (eje Y de la c�mara)
        rotX -= mouseY;
        rotX = Mathf.Clamp(rotX, minLookX, maxLookX); // Limitar el �ngulo de rotaci�n vertical

        // Aplicar la rotaci�n vertical a la c�mara
        playerCamera.transform.localRotation = Quaternion.Euler(rotX, 0f, 0f);

        // Rotaci�n horizontal (eje X del jugador)
        transform.Rotate(Vector3.up * mouseX);
    }
}

