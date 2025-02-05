using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;
    [Header("Movement Settings")]
    public float moveSpeed = 10f; // Speed of left/right movement
    public float moveForwardSpeed = 10f; // Speed of left/right movement

    public float boost_moveSpeed = 20f; // Speed of left/right movement
    public float jumpForce = 8f; // Jump strength
    public float gravity = 20f; // Gravity applied when airborne
    public float rotationSpeed = 5f; // Speed of rotation when moving left/right
    public float maxRotation = 15f; // Maximum rotation angle when moving left/right
    public float GroundCheckRayCastLenght = 1f;


    

    [Header("Slide Settings")]
    public float slideDuration = 0.5f; // Duration of the slide
    private float slideTimer = 0f;

    [Header("Boundaries")]
    public float minX = -5f; // Minimum X position
    public float maxX = 5f; // Maximum X position
    public LayerMask GroundLayer;

    private Vector3 moveDirection = Vector3.zero;
    private CharacterController controller;
    private bool isJumping = false;
    private bool isSliding = false;
    private bool isHurt = false;
    [Header("Animation Settings")]
    private Animator animator;

    [Header("Boost Management")]
    public bool BoostEnabled = false;
    float currSpeed;

    public float getCurrSpeed()
    {
        return currSpeed;
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        animator.SetTrigger("Run"); // Start with running animation
        currSpeed = moveForwardSpeed;
    }

    private void Update()
    {
        if (isHurt)
        {
            return;
        }

        HandleLaneMovement();
        HandleJumpAndSlide();
        ApplyGravity();

        // Move the player using CharacterController
        controller.Move(moveDirection * Time.deltaTime);

        // Manually clamp the X position after movement
        float clampedX = Mathf.Clamp(transform.position.x, minX, maxX);
        transform.position = new Vector3(clampedX, transform.position.y, transform.position.z);
        
        Debug.Log(IsGrounded());
    }

    private void HandleLaneMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal"); // Left/Right input

        // Update movement direction (horizontal input)
        moveDirection.x = horizontalInput * moveSpeed;

        // Smoothly rotate the player in the direction of movement
        if (horizontalInput != 0)
        {
            float targetRotation = maxRotation * Mathf.Sign(horizontalInput); // Rotate left/right
            Quaternion newRotation = Quaternion.Euler(0, targetRotation, 0);
            transform.rotation = Quaternion.Lerp(transform.rotation, newRotation, Time.deltaTime * rotationSpeed);
        }
        else
        {
            // Smoothly return to default rotation when not moving
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.identity, Time.deltaTime * rotationSpeed);
        }
    }

    private void HandleJumpAndSlide()
    {
        bool jumpPressed = Input.GetKeyDown(KeyCode.UpArrow);
        bool slidePressed = Input.GetKeyDown(KeyCode.DownArrow);

        if (IsGrounded())
        {
            isJumping = false;
            moveDirection.y = 0f;
            animator.SetTrigger("Run"); // Return to running when grounded

            if (jumpPressed)
            {
                moveDirection.y = jumpForce;
                isJumping = true;
                animator.SetTrigger("Jump");
            }

            if (slidePressed)
            {
                StartSlide();
            }
        }
        // else
        // {
        //     if (isSliding)
        //     {
        //         EndSlide();
        //     }
        // }

        // if (isSliding)
        // {
        //     slideTimer -= Time.deltaTime;
        //     if (slideTimer <= 0f)
        //     {
        //         EndSlide();
        //     }
        // }
    }

    private void StartSlide()
    {
        isSliding = true;
        slideTimer = slideDuration;
        controller.height = 1f; // Adjust height for sliding
        controller.center = new Vector3(controller.center.x, controller.height / 2, controller.center.z);
        animator.SetTrigger("Slide");
    }

    public void EndSlide() // called in animation event
    {
        Debug.Log("EndRun");
        isSliding = false;
        controller.height = 1.9f; // Reset height after sliding
        controller.center = new Vector3(controller.center.x, controller.height / 2, controller.center.z);
        animator.SetTrigger("Run");
    }

    private void ApplyGravity()
    {
        if (!IsGrounded())
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {
            Hurt();
        }
        else if (other.CompareTag("Collectable"))
        {
            Collect();
        }
    }

    private void Hurt()
    {
        isHurt = true;
        animator.SetTrigger("Hurt");
        ObstacleSpawner.Instance.StopSpawning(false);
        StopAllObstacles(false);
        
        CollectableSpawner.Instance.StopSpawning(false);
        StopAllCollectables(false);
        
        EnvironmentManager.Instance.StopSpawning(false);
        StopAllEnvironment(false);
    }

    private void Collect()
    {
        GameManager.Instance.Collected(1);
    }

    private void StopAllObstacles(bool state)
    {
        Obstacle[] obstacles = FindObjectsOfType<Obstacle>();
        foreach (Obstacle obstacle in obstacles)
        {
            obstacle.StopMovement(state);
        }
    }
    
    private void StopAllCollectables(bool state)
    {
        Collectable[] collectables = FindObjectsOfType<Collectable>();
        foreach (Collectable collectable in collectables)
        {
            collectable.StopMovement(state);
        }
    }
    
    private void StopAllEnvironment(bool state)
    {
        EnvironmentPatch[] environmentPatches = FindObjectsOfType<EnvironmentPatch>();
        foreach (EnvironmentPatch environment in environmentPatches)
        {
            environment.StopMovement(state);
        }
    }

    public void RestHurt() // Called in animation Event
    {
        isHurt = false;
        CollectableSpawner.Instance.StopSpawning(true);
        ObstacleSpawner.Instance.StopSpawning(true);
        StopAllObstacles(true);
        StopAllCollectables(true);
        
        EnvironmentManager.Instance.StopSpawning(true);
        StopAllEnvironment(true);
    }

    private bool IsGrounded()
    {
        float rayLength = GroundCheckRayCastLenght + 0.1f; // Slightly increase length
        Vector3 rayOrigin = transform.position + Vector3.up * 0.1f; // Offset to prevent inside-ground issues

        if (Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit, rayLength, GroundLayer))
        {
            return hit.collider.CompareTag("Ground"); // Optional: Check tag
        }

        return false;
    }
}
