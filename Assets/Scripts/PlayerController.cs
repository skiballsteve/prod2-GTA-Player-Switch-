using Unity.Cinemachine;
using UnityEditor.Animations;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region Inspector Fields

    [Header("Movement")]
    public Animator playerAniController;
    public float moveSpeed = 5f;
    public float sprintSpeed = 8f;
    public float rotationSpeed = 10f;
    public float gravity = -9.81f;
    public float jumpHeight = 2f;

    [Header("Camera")]
    public Transform cameraTarget;
    public CinemachineCamera Playercam;
    public GameObject rayCastObject;
    public Transform axeThrowPoint;
    public Vector3 throwPointVec;
    public Vector3 raycastOffset = Vector3.zero;
    private LayerMask raycastLayers;
    public float maxRayDistance = 1000f;
    public float mouseSensitivity = 2f;
    public float minVerticalAngle = -30f;
    public float maxVerticalAngle = 70f;
    public float cameraDistance = 5f;
    public float cameraHeight = 2f;
    public float cameraXoffSet = 0f;

    [Header("Axes")]
    public GameObject aniAxe;
    public GameObject throwPointReset;

    #endregion

    #region Private Fields

    private CharacterController controller;
    private Vector3 velocity;
    private float cameraPitch = 0f;
    private float cameraYaw = 0f;

    #endregion

    #region Unity Lifecycle

    void Start()
    {
        InitializeController();
        InitializeThrowPoint();
        InitializeRaycastObject();
        InitializeCameraTarget();
        InitializeCursor();
    }

    void Update()
    {
        HandleMovement();
        HandleCamera();
        HandleThrow();
    }

    #endregion

    #region Initialization

    void InitializeController()
    {
        controller = GetComponent<CharacterController>();

        if (controller == null)
        {
            controller = gameObject.AddComponent<CharacterController>();
            controller.height = 2f;
            controller.radius = 0.5f;
            controller.center = new Vector3(0, 1f, 0);
        }
    }

    void InitializeThrowPoint()
    {
        throwPointVec = axeThrowPoint.transform.localPosition;
    }

    void InitializeRaycastObject()
    {
        rayCastObject.transform.localPosition = new Vector3(0.17f, 0.32f, 4.24f);
        rayCastObject.transform.localRotation = Quaternion.Euler(-7, 0, 0);
    }

    void InitializeCameraTarget()
    {
        if (cameraTarget == null)
        {
            GameObject target = new GameObject("CameraTarget");
            cameraTarget = target.transform;
            cameraTarget.parent = transform;
            cameraTarget.localPosition = new Vector3(0, 1.6f, 0);
        }

        cameraYaw = transform.eulerAngles.y;
    }

    void InitializeCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    #endregion

    #region Movement

    void HandleMovement()
    {
        bool isGrounded = controller.isGrounded;

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 moveDir = CalculateMoveDirection(h, v);
        ProcessMovement(moveDir, isGrounded);
        ProcessJump(isGrounded);
        ApplyGravity();
        HandleEscapeKey();
    }

    Vector3 CalculateMoveDirection(float horizontal, float vertical)
    {
        Vector3 forward = Playercam.transform.forward;
        Vector3 right = Playercam.transform.right;
        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();

        return forward * vertical + right * horizontal;
    }

    void ProcessMovement(Vector3 moveDir, bool isGrounded)
    {
        playerAniController.SetBool("IsWalking", true);

        if (moveDir.magnitude > 0.1f)
        {
            playerAniController.SetBool("IsWalking", true);
            float speed = DetermineMovementSpeed();
            controller.Move(moveDir.normalized * speed * Time.deltaTime);

            if (Input.GetButton("Fire2"))
            {
                HandleStrafingAnimations();
            }
            else
            {
                RotatePlayerToMovement(moveDir);
                ResetStrafingAnimations();
            }
        }
        else
        {
            playerAniController.SetBool("IsWalking", false);
            playerAniController.SetBool("IsRunning", false);
        }
    }

    float DetermineMovementSpeed()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            playerAniController.SetBool("IsRunning", true);
            return sprintSpeed;
        }
        else
        {
            playerAniController.SetBool("IsRunning", false);
            return moveSpeed;
        }
    }

    void HandleStrafingAnimations()
    {
        if (Input.GetKey(KeyCode.D))
        {
            playerAniController.SetBool("IsWalking", false);
            playerAniController.SetBool("IsMoveRight", true);
        }
        else
        {
            playerAniController.SetBool("IsMoveRight", false);
        }

        if (Input.GetKey(KeyCode.A))
        {
            playerAniController.SetBool("IsWalking", false);
            playerAniController.SetBool("IsMoveLeft", true);
        }
        else
        {
            playerAniController.SetBool("IsMoveLeft", false);
        }

        if (Input.GetKey(KeyCode.W))
        {
            playerAniController.SetBool("IsWalking", false);
            playerAniController.SetBool("IsMoveForward", true);
        }
        else
        {
            playerAniController.SetBool("IsMoveForward", false);
        }

        if (Input.GetKey(KeyCode.S))
        {
            playerAniController.SetBool("IsWalking", false);
            playerAniController.SetBool("IsMoveBack", true);
        }
        else
        {
            playerAniController.SetBool("IsMoveBack", false);
        }
    }

    void RotatePlayerToMovement(Vector3 moveDir)
    {
        Quaternion targetRotation = Quaternion.LookRotation(moveDir);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    void ResetStrafingAnimations()
    {
        playerAniController.SetBool("IsMoveRight", false);
        playerAniController.SetBool("IsMoveLeft", false);
        playerAniController.SetBool("IsMoveForward", false);
        playerAniController.SetBool("IsMoveBack", false);
    }

    void ProcessJump(bool isGrounded)
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }

    void ApplyGravity()
    {
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    void HandleEscapeKey()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    #endregion

    #region Camera

    void HandleCamera()
    {
        UpdateCameraRotation();

        Quaternion rotation = Quaternion.Euler(cameraPitch, cameraYaw, 0);
        Vector3 offset = rotation * new Vector3(cameraXoffSet, cameraHeight, -cameraDistance);

        if (Input.GetButton("Fire2"))
        {
            HandleAimMode(offset);
        }
        else
        {
            HandleNormalMode(offset);
        }

        if (Input.GetButtonUp("Fire2"))
        {
            ResetAxeThrowPoint();
        }
    }

    void UpdateCameraRotation()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        cameraYaw += mouseX;
        cameraPitch -= mouseY;
        cameraPitch = Mathf.Clamp(cameraPitch, minVerticalAngle, maxVerticalAngle);
    }

    void HandleAimMode(Vector3 offset)
    {
        ProcessRaycastAiming();
        PositionAndRotateCamera(offset);
        RotatePlayerToCamera();
    }

    void ProcessRaycastAiming()
    {
        int layerToIgnore = LayerMask.GetMask("Ignore Raycast");
        int layerMask = ~layerToIgnore;

        Vector3 rayOrigin = Playercam.transform.position +
                           Playercam.transform.right * raycastOffset.x +
                           Playercam.transform.up * raycastOffset.y +
                           Playercam.transform.forward * raycastOffset.z;

        Ray ray = new Ray(rayCastObject.transform.position, rayCastObject.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxRayDistance, layerMask))
        {
            Vector3 directionToHit = hit.point - axeThrowPoint.position;
            if (directionToHit != Vector3.zero)
            {
                axeThrowPoint.rotation = Quaternion.LookRotation(directionToHit);
            }
        }

        Debug.DrawRay(ray.origin, ray.direction * maxRayDistance, Color.red);
        Debug.DrawLine(Playercam.transform.position, ray.origin, Color.yellow);
    }

    void PositionAndRotateCamera(Vector3 offset)
    {
        Playercam.transform.position = cameraTarget.position + offset;
        Playercam.transform.LookAt(cameraTarget.position);
    }

    void RotatePlayerToCamera()
    {
        float turnSpeed = 10f;
        Vector3 camEuler = Playercam.transform.eulerAngles;
        camEuler.x = transform.eulerAngles.x;

        Quaternion targetRot = Quaternion.Euler(camEuler);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * turnSpeed);
    }

    void HandleNormalMode(Vector3 offset)
    {
        Playercam.transform.position = cameraTarget.position + offset;
        Playercam.transform.LookAt(cameraTarget.position);
    }

    void ResetAxeThrowPoint()
    {
        axeThrowPoint.transform.localPosition = throwPointVec;
        axeThrowPoint.rotation = throwPointReset.transform.rotation;
    }

    #endregion

    #region Combat

    public void HandleThrow()
    {
        if (Input.GetButtonDown("Fire1") && aniAxe.activeSelf)
        {
            playerAniController.SetBool("IsThrowing", true);
        }
        else
        {
            playerAniController.SetBool("IsThrowing", false);
        }
    }

    #endregion
}
