using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class FirstPersonController : MonoBehaviour
{
    public bool CanMove { get; private set; } = true;
    private bool IsSprinting => canSprint && Input.GetKey(sprintKey);
    private bool ShouldJump => characterController.isGrounded && Input.GetKeyDown(jumpKey);
    private bool ShouldCrouch => characterController.isGrounded && Input.GetKeyDown(crouchKey);

    [Header("Functional Options")]
    [SerializeField] private bool canSprint = true;
    [SerializeField] private bool canJump = true;
    [SerializeField] private bool canCrouch = true;
    [SerializeField] private bool canUseHeadbob= true;
    [SerializeField] private bool canInteract = true;
    [SerializeField] private bool canZoom = true;

    [Header("Controls")]
    [SerializeField] private KeyCode sprintKey = KeyCode.LeftShift;
    [SerializeField] private KeyCode jumpKey = KeyCode.Space;
    [SerializeField] private KeyCode crouchKey = KeyCode.LeftControl;
    [SerializeField] private KeyCode interactKey = KeyCode.Mouse0;
    [SerializeField] private KeyCode zoomKey = KeyCode.Mouse1;

    [Header("Movement Parameters")]
    [SerializeField] private float walkSpeed = 3.0f;
    [SerializeField] private float sprintSpeed = 6.0f;
    [SerializeField] private float crouchSpeed = 1.5f;

    [Header("Mouse Look Parameters")]
    [SerializeField,Range(1,10)] private float lookSpeedX = 2.0f;
    [SerializeField,Range(1,10)] private float lookSpeedY = 2.0f;
    [SerializeField,Range(1,180)] private float upperLookLimit = 80.0f;
    [SerializeField,Range(1,180)] private float lowerLookLimit = 80.0f;

    [Header("Jumping Parameters")]
    [SerializeField] private float jumpForce = 8.0f;
    [SerializeField] private float gravity = 30.0f;

    [Header("Crouch Parametes")]
    [SerializeField] private float crouchHeight = 0.5f;
    [SerializeField] private float standHeight = 2.0f;
    [SerializeField] private float timeToCrouch = 0.25f;
    [SerializeField] private Vector3 crouchingCenter = new Vector3(0, 0.5f, 0);
    [SerializeField] private Vector3 standingCenter = new Vector3(0, 0, 0);
    private bool isCrouching;

    [Header("Headbobbing Parameters")]
    [SerializeField] private float walkBobSpeed = 14f;
    [SerializeField] private float walkBobAmount = 0.05f;
    [SerializeField] private float sprintBobSpeed = 20f;
    [SerializeField] private float sprintBobAmount = 0.1f;
    [SerializeField] private float crouchBobSpeed = 10f;
    [SerializeField] private float crouchBobAmount = 0.025f;
    private float defaultPosY;
    private float timer;

    [Header("Zoom")]
    [SerializeField] private float timeToZoom = 0.3f;
    [SerializeField] private float zoomFOV = 30f;
    private float defaultFOV;
    private Coroutine zoomCoroutine;

    [Header("Interaction")]
    [SerializeField] private Vector3 interactionRayPoint = default;
    [SerializeField] private float interactionDistance = default;
    [SerializeField] private LayerMask interactionLayer = default;
    private Interactable currentInteractable;


    private Camera playerCamera;
    private CharacterController characterController;

    private Vector3 moveDirection;
    private Vector2 currentInput;

    private float rotationX = 0;

    //ENVANTERE BATARYA ALMA
    private bool battery;
    public GameObject batteryObject;
    public Animator freddyAnimator;
    public Text youWin;

    //ENVANTERE GALAXYMAP ALMA
    public bool galaxymap;
    public GameObject galaxymapobject;

    //ENVANTERE EGYPTMAP ALMA
    public bool egyptmap;
    public GameObject egyptmapobject;

    void Awake()
    {
        playerCamera = GetComponentInChildren<Camera>();
        defaultPosY = playerCamera.transform.localPosition.y;
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        defaultFOV = playerCamera.fieldOfView;
        battery = false;
        galaxymap = false;
        egyptmap = false;
        youWin.gameObject.SetActive(false);
    }
    void Update()
    {
        if(CanMove)
        {
            HandleMovementInput();
            HandleMouseLook();
            ApplyFinalMovements();

            if (canJump)
            {
                HandleJump();
            }

            if (canCrouch)
            {
                HandleCrouch();
            }

            if(canUseHeadbob)
            {
                HandleHeadbob();
            }

            if(canInteract)
            {
                HandleInteractionCheck();
                HandleInteractionInput();
            }

            if(canZoom)
            {
                HandleZoom();
            }
        }



        //ENVANTERE BATARYA ALMA/KULLANMA

        if (Input.GetMouseButtonDown(0)) // Change 0 to 1 for right mouse button
        {

            // Get the main camera
            Camera mainCamera = Camera.main;

            // Check if the main camera exists
            if (mainCamera != null)
            {
                // Get the mouse position
                Vector3 mousePosition = Input.mousePosition;

                // Create a ray from the camera through the mouse position
                Ray ray = mainCamera.ScreenPointToRay(mousePosition);

                // Create a RaycastHit variable to store information about the hit
                RaycastHit hit;

                // Perform the raycast
                if (Physics.Raycast(ray, out hit))
                {
                    // If the ray hits something, you can access information about the hit
                    string hitname = hit.collider.gameObject.name;

                    //batarya varsa topla, batarya varken freddye basınca oyunu bitirme animasyonunu oynat
                    if (hitname == "Battery")
                    {
                        battery = true;
                        Destroy(batteryObject);
                    }
                    else if (hitname == "Freddy" && battery)
                    {
                        Debug.Log("freddy çalışmaya başladı");
                        freddyAnimator.SetBool("EnableFreddy", true);
                        youWin.gameObject.SetActive(true);
                    }
                    else if (hitname == "Galaxymap")
                    {
                        galaxymap = true;
                        Destroy(galaxymapobject);

                        //I TUŞU İLE GALAKSİ ÇAĞINA GİDEBİLİRSİN MESAJI
                    }
                    else if (hitname == "Egyptmap")
                    {
                        egyptmap = true;
                        Destroy(egyptmapobject);

                        //U TUŞU İLE GALAKSİ ÇAĞINA GİDEBİLİRSİN MESAJI
                    }
                }
                else
                {
                    // If the ray doesn't hit anything, you can handle this case here
                    Debug.Log("No object hit.");
                }
            }
        }
    }

    private void HandleZoom()
    {
        if(Input.GetKeyDown(zoomKey))
        {
            if (zoomCoroutine != null)
            {
                StopCoroutine(zoomCoroutine);
                zoomCoroutine = null;
            }

            zoomCoroutine = StartCoroutine(Zoom(true));
        }
        else if(Input.GetKeyUp(zoomKey))
        {
            if (zoomCoroutine != null)
            {
                StopCoroutine(zoomCoroutine);
                zoomCoroutine = null;
            }

            zoomCoroutine = StartCoroutine(Zoom(false));
        }
    }

    IEnumerator Zoom(bool isEnter)
    {
        float targetFov = isEnter ? zoomFOV : defaultFOV;
        float startFov = playerCamera.fieldOfView;
        float timeElapsed = 0;

        while(timeElapsed<timeToZoom)
        {
            playerCamera.fieldOfView = Mathf.Lerp(startFov, targetFov, timeElapsed / timeToZoom);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        playerCamera.fieldOfView = targetFov;
        zoomCoroutine = null;
    }

    private void ApplyFinalMovements()
    {
        if(!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }
        characterController.Move(moveDirection * Time.deltaTime);
    }

    private void HandleMouseLook()
    {
        rotationX -= Input.GetAxis("Mouse Y") * lookSpeedY;
        rotationX = Mathf.Clamp(rotationX, -upperLookLimit, lowerLookLimit);
        playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
        transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeedX, 0);
    }

    private void HandleJump()
    {
        if (ShouldJump)
        {
            moveDirection.y = jumpForce;
        }
    }

    private void HandleHeadbob()
    {
        if (!characterController.isGrounded) { return; }

        if(MathF.Abs(moveDirection.x) > 0.1f|| MathF.Abs(moveDirection.z) > 0.1f)
        {
            timer += Time.deltaTime * (isCrouching ? crouchBobSpeed : IsSprinting ? sprintBobSpeed : walkBobSpeed);
            playerCamera.transform.localPosition = new Vector3(playerCamera.transform.localPosition.x, defaultPosY + Mathf.Sin(timer) * (isCrouching ? crouchBobAmount : IsSprinting ? sprintBobAmount : walkBobAmount), playerCamera.transform.localPosition.z);
        }
    }

    private void HandleInteractionInput()
    {
        if(Input.GetKeyDown(interactKey) && currentInteractable != null && Physics.Raycast(playerCamera.ViewportPointToRay(interactionRayPoint),out RaycastHit hit, interactionDistance, interactionLayer))
        {
            currentInteractable.OnInteract();
        }
        if (Input.GetKey(interactKey) && currentInteractable != null && Physics.Raycast(playerCamera.ViewportPointToRay(interactionRayPoint), out RaycastHit hita, interactionDistance, interactionLayer))
        {
            currentInteractable.OnHoldInteract();
        }
    }

    private void HandleInteractionCheck()
    {
        if (Physics.Raycast(playerCamera.ViewportPointToRay(interactionRayPoint), out RaycastHit hit, interactionDistance))
        {
            if(hit.collider.gameObject.layer == 9 && (currentInteractable == null||hit.collider.gameObject.GetInstanceID() != currentInteractable.GetInstanceID()))
            {
                hit.collider.TryGetComponent(out currentInteractable);

                if(currentInteractable)
                {
                    currentInteractable.OnFocus();
                }
            }
        }
        else if(currentInteractable)
        {
            currentInteractable.OnLoseFocus();
            currentInteractable = null;
        }
    }

    private void HandleCrouch()
    {
        if(ShouldCrouch)
        {
            StartCoroutine(CrouchStand());
        }
    }

    IEnumerator CrouchStand()
    {
        if(isCrouching && Physics.Raycast(playerCamera.transform.position, Vector3.up, 1f))
            yield break;

        float timeElapsed = 0;
        float targetHeight = isCrouching ? standHeight : crouchHeight;
        float currentHeight = characterController.height;
        Vector3 targetCenter = isCrouching ? standingCenter : crouchingCenter;
        Vector3 currentCenter = characterController.center;

        while (timeElapsed < timeToCrouch)
        {
            characterController.height = Mathf.Lerp(currentHeight, targetHeight, timeElapsed / timeToCrouch);
            characterController.center = Vector3.Lerp(currentCenter, targetCenter, timeElapsed / timeToCrouch);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        characterController.height = targetHeight;
        characterController.center = targetCenter;

        isCrouching = !isCrouching;
    }

    private void HandleMovementInput()
    {
        currentInput = new Vector2((isCrouching ? crouchSpeed :IsSprinting ? sprintSpeed : walkSpeed) * Input.GetAxis("Vertical"), (isCrouching ? crouchSpeed:IsSprinting ? sprintSpeed : walkSpeed) * Input.GetAxis("Horizontal"));
        float moveDirectionY = moveDirection.y;
        moveDirection = (transform.TransformDirection(Vector3.forward) * currentInput.x) + (transform.TransformDirection(Vector3.right) * currentInput.y);
        moveDirection.y = moveDirectionY;
    }
}
