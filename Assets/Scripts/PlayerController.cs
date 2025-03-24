using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;

    [Header("Movement Settings")] public float moveSpeed = 10f;
    public float moveForwardSpeed = 10f;
    public float moveForwardBoostSpeed = 10f;
    public float boost_moveSpeed = 20f;
    public float jumpForce = 8f;
    public float gravity = 20f;
    public float rotationSpeed = 5f;
    public float maxRotation = 15f;
    public float groundCheckDistance = 0.2f; // Reduced to a more reasonable value
    [HideInInspector] public float horizontalInput;

    [Header("Slide Settings")] public float slideDuration = 0.5f;
    private float slideTimer = 0f;

    [Header("Boundaries")] public float minX = -5f;
    public float maxX = 5f;
    public LayerMask groundLayer;

    [HideInInspector] public Vector3 moveDirection = Vector3.zero;
    [SerializeField] public CharacterController controller;
    private bool isJumping = false;
    private bool isSliding = false;
    [HideInInspector] public bool canMovement;
    private bool isHurt = false;

    [Header("Animation Settings")] public Animator animator;

    [Header("Boost Management")] public bool BoostEnabled = false;
    public float BoostTimer = 5f;
    private float currSpeed;

    public bool IsMagnetOn = false;
    public Transform CashPoint;
    public Button MagnetButton;
    public TextMeshProUGUI MagnetText;
    public BoxCollider CashCollider;

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
        animator.SetTrigger("Run");
        MagnetButton.onClick.AddListener(ToggleMagnet);
        canMovement = true;
    }

    private void Update()
    {
        if (isHurt) return;

        HandleLaneMovement();
        HandleJumpAndSlide();
        ApplyGravity();
        StopBooster();

        currSpeed = moveForwardSpeed;

        // Move the player using CharacterController
        controller.Move(moveDirection * Time.deltaTime);

        // Clamp the X position after movement
        float clampedX = Mathf.Clamp(transform.position.x, minX, maxX);
        transform.position = new Vector3(clampedX, transform.position.y, transform.position.z);

        if (Input.GetKeyDown(KeyCode.B))
            BoostEnabled = !BoostEnabled;
    }

    private void HandleLaneMovement()
    {
        if (canMovement)
        {
            horizontalInput = Input.GetAxis("Horizontal");
            moveDirection.x = horizontalInput * moveSpeed;

            if (horizontalInput != 0)
            {
                float targetRotation = maxRotation * Mathf.Sign(horizontalInput);
                Quaternion newRotation = Quaternion.Euler(0, targetRotation, 0);
                transform.rotation = Quaternion.Lerp(transform.rotation, newRotation, Time.deltaTime * rotationSpeed);
            }
            else
            {
                transform.rotation =
                    Quaternion.Lerp(transform.rotation, Quaternion.identity, Time.deltaTime * rotationSpeed);
            }
        }
    }

    private void HandleJumpAndSlide()
    {
        if (canMovement)
        {
            bool jumpPressed = Input.GetKeyDown(KeyCode.UpArrow);
            bool slidePressed = Input.GetKeyDown(KeyCode.DownArrow);

            if (IsGrounded())
            {
                isJumping = false;
                moveDirection.y = -0.1f; // Keep grounded

                animator.SetTrigger("Run");

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
        }
    }

    private void ToggleMagnet()
    {
        IsMagnetOn = !IsMagnetOn;
        CashCollider.size = IsMagnetOn ? new Vector3(500, 50, 1) : new Vector3(1, 50, 1);
        MagnetText.text = IsMagnetOn ? "On" : "Off";
    }

    private void StartSlide()
    {
        isSliding = true;
        slideTimer = slideDuration;
        controller.height = 1f;
        controller.center = new Vector3(controller.center.x, controller.height / 2, controller.center.z);
        animator.SetTrigger("Slide");
    }

    public void EndSlide() // Called in animation event
    {
        isSliding = false;
        controller.height = 2.22f;
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
            Collect(other.gameObject.GetComponent<Collectable>().myid);
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

        Audiomanager.instance.PlaySfx_Collision();
    }

    private void Collect(int id)
    {
        GameManager.Instance.Collected(1, id);
        Audiomanager.instance.PlaySfx_Coins();
    }

    private void StopAllObstacles(bool state)
    {
        foreach (Obstacle obstacle in FindObjectsOfType<Obstacle>())
        {
            obstacle.StopMovement(state);
        }
    }

    private void StopAllCollectables(bool state)
    {
        foreach (Collectable collectable in FindObjectsOfType<Collectable>())
        {
            collectable.StopMovement(state);
        }
    }

    private void StopAllEnvironment(bool state)
    {
        foreach (EnvironmentPatch environment in FindObjectsOfType<EnvironmentPatch>())
        {
            environment.StopMovement(state);
        }
    }

    public void StopBooster()
    {
        if (BoostEnabled)
        {
            BoostTimer -= Time.deltaTime;
            if (BoostTimer <= 0)
            {
                BoostEnabled = false;
                BoostTimer = 5;
            }
        }
    }

    public void RestHurt() // Called in animation event
    {
        isHurt = false;
        CollectableSpawner.Instance.StopSpawning(true);
        ObstacleSpawner.Instance.StopSpawning(true);
        StopAllObstacles(true);
        StopAllCollectables(true);
        EnvironmentManager.Instance.StopSpawning(true);
        StopAllEnvironment(true);
        ResetStumble();
    }

    public void ResetStumble()
    {
        canMovement = true;
        moveForwardSpeed = 20f;
        moveSpeed = 7f;
    }

    private bool IsGrounded()
    {
        return controller.isGrounded; // Use CharacterController's built-in ground detection
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, Vector3.down * groundCheckDistance);
    }
}