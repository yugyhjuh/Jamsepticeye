using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 3f; // Walking speed
    public float sprintSpeed = 6f;  // Sprint speed
    public float mouseSensitivity = 2f; // Camera sensitivity
    public float maxLookX = 80f; // Look up limit
    public float minLookX = -80f; // Look down limit
    public KeyCode sprintKey = KeyCode.LeftShift;
    private float currentSpeed;

    [Header("Stamina Settings")]
    public float maxStamina = 5f;          // Max stamina in seconds
    public float stamina;                  // Current stamina
    public float staminaDrainRate = 1f;    // How fast it drains while sprinting
    public float staminaRegenRate = 0.5f;  // How fast it regenerates when not sprinting
    private bool isSprinting = false;

    private float rotX; // Camera rotation on X axis
    private CharacterController controller;
    private Camera playerCamera;
    private float originalCameraY;

    [Header("Jump & Gravity")]
    public float jumpHeight = 1.5f;
    public float gravity = -9.81f;
    private float verticalVelocity;

    [Header("Crouch")]
    public float crouchHeight = 1f;
    public float standHeight = 2f;
    public float crouchSpeed = 2f; // speed for smooth crouch transition
    public float crouchMovementSpeed = 1.5f; //speed while croucghed
    private bool isCrouching = false;





    [Header("Interaction Settings")]
    public float interactDistance = 3f; // How far player can interact
    public LayerMask interactMask;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        playerCamera = Camera.main;
        originalCameraY = playerCamera.transform.localPosition.y;

        // Lock cursor for immersion
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        CameraLook();
        Interact();
        HandleCrouch();
    }

    void Move()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        // Sprint
        
        float currentSpeed = isCrouching
            ? crouchMovementSpeed
            : (Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed);


        // Horizontal movement
        Vector3 move = transform.right * x + transform.forward * z;

        // Check if player can sprint
        if (!isCrouching && Input.GetKey(KeyCode.LeftShift) && stamina > 0)
        {
            isSprinting = true;
            currentSpeed = sprintSpeed;
            stamina -= staminaDrainRate * Time.deltaTime; // drain stamina
        }
        else
        {
            isSprinting = false;
            currentSpeed = walkSpeed;

            // Regenerate stamina when not sprinting
            if (stamina < maxStamina)
                stamina += staminaRegenRate * Time.deltaTime;
        }

        stamina = Mathf.Clamp(stamina, 0, maxStamina); // prevent going below 0 or above max

        // Gravity
        if (controller.isGrounded && verticalVelocity < 0)
            verticalVelocity = -2f; // small negative to keep grounded

        // Jump
        if (Input.GetKeyDown(KeyCode.Space) && controller.isGrounded)
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);

        verticalVelocity += gravity * Time.deltaTime;
        move.y = verticalVelocity;

        controller.Move(move * currentSpeed * Time.deltaTime);
    }

    void HandleCrouch()
    {
        // Toggle crouch
        if (Input.GetKeyDown(KeyCode.LeftControl))
            isCrouching = !isCrouching;

        // Smoothly lerp height
        float targetHeight = isCrouching ? crouchHeight : standHeight;
        controller.height = Mathf.Lerp(controller.height, targetHeight, crouchSpeed * Time.deltaTime);

        // Adjust center so capsule stays grounded
        Vector3 center = controller.center;
        center.y = controller.height / 2f;
        controller.center = center;

        // Adjust camera to match crouch
        Vector3 camPos = playerCamera.transform.localPosition;
        camPos.y = controller.height - 0.2f; // offset inside capsule
        playerCamera.transform.localPosition = Vector3.Lerp(playerCamera.transform.localPosition, camPos, crouchSpeed * Time.deltaTime);

/*        if (isCrouching)
        {
            currentSpeed = crouchMovementSpeed;
        }
        if (!isCrouching)
        {
            currentSpeed = walkSpeed;
        }*/
    }

    void CameraLook()
    {
        float y = Input.GetAxis("Mouse X") * mouseSensitivity;
        rotX += Input.GetAxis("Mouse Y") * mouseSensitivity;

        rotX = Mathf.Clamp(rotX, minLookX, maxLookX);

        // Rotate camera up/down
        playerCamera.transform.localRotation = Quaternion.Euler(-rotX, 0, 0);
        // Rotate body left/right
        transform.Rotate(Vector3.up * y);
    }

    void Interact()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, interactDistance, interactMask))
            {
                Interactable interactable = hit.collider.GetComponent<Interactable>();
                if (interactable != null)
                {
                    interactable.Interact();
                }
            }
        }
    }
}
