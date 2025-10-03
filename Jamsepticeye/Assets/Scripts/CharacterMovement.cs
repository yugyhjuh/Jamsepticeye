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
    [SerializeField] private float stamina;                  // Current stamina
    public float staminaDrainRate = 1f;    // How fast it drains while sprinting
    public float staminaRegenRate = 0.5f;  // How fast it regenerates when not sprinting
    //private bool isSprinting = false;
    private bool canSprint = true;


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


/*    [Header("Interaction Settings")]
    public float interactDistance = 3f; // How far player can interact
    public LayerMask interactMask;*/




    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        playerCamera = Camera.main;
        originalCameraY = playerCamera.transform.localPosition.y;
        stamina = maxStamina;

        // Lock cursor for immersion
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        CameraLook();
        //Interact();
        HandleCrouch();

        //Debug.Log("Current Stamina: " + stamina.ToString("F2"));
    }

    void Move()
    {
        // Get input
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        // Calculate movement direction
        Vector3 move = transform.right * x + transform.forward * z;

        // Determine movement speed
        if (isCrouching)
        {
            currentSpeed = crouchMovementSpeed;
        }
        else
        {
            if (Input.GetKey(KeyCode.LeftShift) && canSprint)
            {
                currentSpeed = sprintSpeed;
                stamina -= staminaDrainRate * Time.deltaTime;

                // Disable sprint if stamina drops below 0
                if (stamina <= 0)
                {
                    stamina = 0;
                    canSprint = false;
                }
            }
            else
            {
                currentSpeed = walkSpeed;

                // Regenerate stamina
                if (stamina < maxStamina)
                {
                    stamina += staminaRegenRate * Time.deltaTime;

                    // Once fully recharged, allow sprint again
                    if (stamina >= maxStamina)
                        canSprint = true;
                }
            }
        }

        // Clamp stamina to stay within limits
        stamina = Mathf.Clamp(stamina, 0, maxStamina);

        // Gravity
        if (controller.isGrounded && verticalVelocity < 0)
            verticalVelocity = -2f; // keep grounded

        // Jump
        if (Input.GetKeyDown(KeyCode.Space) && controller.isGrounded)
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);

        verticalVelocity += gravity * Time.deltaTime;
        move.y = verticalVelocity;

        // Apply movement
        controller.Move(move * currentSpeed * Time.deltaTime);
    }


    void HandleCrouch()
    {
        // Toggle crouch
        if (Input.GetKeyDown(KeyCode.LeftControl))
            isCrouching = !isCrouching;

        // Target capsule height
        float targetHeight = isCrouching ? crouchHeight : standHeight;

        // Smoothly interpolate capsule height
        controller.height = Mathf.Lerp(controller.height, targetHeight, crouchSpeed * Time.deltaTime);

        // Keep capsule grounded
        Vector3 center = controller.center;
        center.y = controller.height / 2f;
        controller.center = center;

        // Smoothly move camera with capsule
        Vector3 camPos = playerCamera.transform.localPosition;
        float targetCamY = isCrouching ? crouchHeight - 0.2f : standHeight - 0.2f;
        camPos.y = Mathf.Lerp(camPos.y, targetCamY, crouchSpeed * Time.deltaTime);
        playerCamera.transform.localPosition = camPos;

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

/*    void Interact()
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
    }*/

}
