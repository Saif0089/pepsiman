using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;

    [Header("Movement Settings")]
    public float moveSpeed = 10f;
    public float moveForwardSpeed = 10f;
    public float boost_moveSpeed = 20f;
    public float jumpForce = 8f;
    public float rotationSpeed = 5f;
    public float maxRotation = 15f;
    public float GroundCheckRayCastLenght = 1f;
    public LayerMask GroundLayer;

    [Header("Slide Settings")]
    public float slideDuration = 0.5f;
    private float slideTimer = 0f;

    [Header("Boundaries")]
    public float minX = -5f;
    public float maxX = 5f;

    [Header("Animation Settings")]
    public Animator animator;

    [Header("Boost Management")]
    public bool BoostEnabled = false;
    public float BoostTimer = 5f;
    public float SkateTimer = 20f;
    private float currSpeed;

    public float slideColliderHeight = 0.4667208f;
    public GameObject SkateBoard;

    public bool IsMagnetOn = false;
    public bool IsSkateBoardOn = false;

    public Transform CashPoint;
    public Button MagnetButton;
    public TextMeshProUGUI MagnetText;
    public BoxCollider CashCollider;
    public BoxCollider PlayerCollider;

    private Rigidbody rb;
    private bool isGrounded;
    private bool isJumping = false;
    private bool isSliding = false;
    public bool canMovement = true;
    public bool isHurt = false;

    [Header("Start mach")]
    public int StartingCountDown;
    public TextMeshProUGUI StartingCountDownText;
    private Coroutine countdownCoroutine;

    private void Awake()
    {
        if (instance == null)
            instance = this;

        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        animator = GetComponent<Animator>();
        animator.SetTrigger("Run");
        MagnetButton.onClick.AddListener(ToggleMagnet);
        rb.useGravity = true;
        
        StopPlayerAtStart();
    }
    
    public void StartCountdown()
    {
        if (countdownCoroutine != null)
        {
            StopCoroutine(countdownCoroutine);
        }
        countdownCoroutine = StartCoroutine(StartCountDownTime());
    }

    public void StopCountdown()
    {
        if (countdownCoroutine != null)
        {
            StopCoroutine(countdownCoroutine);
            countdownCoroutine = null;
        }
    }
    public IEnumerator StartCountDownTime()
    {
        float currentTime = StartingCountDown;

        while (currentTime > 0)
        {
            currentTime -= Time.deltaTime;
            if (StartingCountDownText != null)
                StartingCountDownText.text = Mathf.Max(0, Mathf.Ceil(currentTime)).ToString("0");

            yield return null;
        }

        if (StartingCountDownText != null)
        {
            StartingCountDownText.text = "Go";
            yield return new WaitForSeconds(1f); 
            StartingCountDownText.gameObject.SetActive(false);
        }

        StartGameNow();
        StopCountdown();
    }
    private void Update()
    {
        if (isHurt) return;

        HandleLaneMovement();
        HandleJumpAndSlide();
        StopBooster();

        if (IsSkateBoardOn)
        {
            SkateTimer -= Time.deltaTime;
            if (SkateTimer <= 0f)
                StopSkate();
        }

        ClampXPosition();

    }
    void FixedUpdate()
    {
        currSpeed = moveForwardSpeed;
    }
    void HandleLaneMovement()
    {
        if (!canMovement) return;

        float horizontalInput = Input.GetAxis("Horizontal");
        Vector3 newVelocity = rb.velocity;
        newVelocity.x = horizontalInput * moveSpeed;
        rb.velocity = newVelocity;

        if (horizontalInput != 0)
        {
            float targetRotation = maxRotation * Mathf.Sign(horizontalInput);
            Quaternion newRotation = Quaternion.Euler(0, targetRotation, 0);
            transform.rotation = Quaternion.Lerp(transform.rotation, newRotation, Time.deltaTime * rotationSpeed);
        }
        else
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.identity, Time.deltaTime * rotationSpeed);
        }
    }

    void HandleJumpAndSlide()
    {
        isGrounded = IsGrounded();

        if (!canMovement || !isGrounded) return;

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            isJumping = true;
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z); // reset Y velocity
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            animator.SetBool("Jump",true);
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            StartSlide();
        }
    }

    void ClampXPosition()
    {
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        transform.position = pos;
    }

    public void ToggleMagnet()
    {
        IsMagnetOn = !IsMagnetOn;

        if (IsMagnetOn)
        {
            CashCollider.size = new Vector3(500, 50, 1);
            MagnetText.text = "On";
        }
        else
        {
            CashCollider.size = new Vector3(1, 50, 1);
            MagnetText.text = "Off";
        }
    }

    void StartSlide()
    {
        PlayerCollider.center = new Vector3(0f, 0.2507838f, 0.1132071f);
        PlayerCollider.size = new Vector3(1, 0.5066212f, 1.00319f);
        isSliding = true;
        slideTimer = slideDuration;
        animator.SetTrigger("Slide");
    }

    public void EndSlide() // Called from animation event
    {
        isSliding = false;
        PlayerCollider.center = new Vector3(0f, 0.9869743f, 0.1132071f);
        PlayerCollider.size = new Vector3(1, 1.979002f, 1.00319f);
        animator.SetTrigger("Run");
    }

    private bool IsGrounded()
    {
        Vector3 origin = transform.position + Vector3.up * 0.1f;
        animator.SetBool("Jump",false);
        return Physics.Raycast(origin, Vector3.down, GroundCheckRayCastLenght, GroundLayer);
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

    void StopAllObstacles(bool state)
    {
        foreach (var obstacle in FindObjectsOfType<Obstacle>())
            obstacle.StopMovement(state);
    }

    void StopAllCollectables(bool state)
    {
        foreach (var collectable in FindObjectsOfType<Collectable>())
            collectable.StopMovement(state);
    }

    void StopAllEnvironment(bool state)
    {
        foreach (var patch in FindObjectsOfType<EnvironmentPatch>())
            patch.StopMovement(state);
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

    void StopPlayerAtStart()
    {
        moveForwardSpeed = 0f;
        GameManager.Instance.gameEnded = true;
        canMovement = false;
        GameManager.Instance.progressBar.transform.parent.gameObject.SetActive(false);
        GameManager.Instance.TimerText.gameObject.SetActive(false);
    }
    void StartGameNow()
    {
        animator.SetBool("Transit",true);
        moveForwardSpeed = 25f;
        GameManager.Instance.gameEnded = false;
        canMovement = true;
        GameManager.Instance.progressBar.transform.parent.gameObject.SetActive(true);
        GameManager.Instance.TimerText.gameObject.SetActive(true);
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
        isHurt = false;
        canMovement = true;
        moveForwardSpeed = 25;
        moveSpeed = 7f;
        PlayerCollider.center = new Vector3(0f, 0.9869743f, 0.1132071f);
        PlayerCollider.size = new Vector3(1, 1.979002f, 1.00319f);
    }

    void StopSkate()
    {
        PlayerCollider.center = new Vector3(0f, 0.9869743f, 0.1132071f);
        PlayerCollider.size = new Vector3(1, 1.979002f, 1.00319f);
        GroundCheckRayCastLenght = 0.25f;
        IsSkateBoardOn = false;
        ToggleMagnet();
        SkateBoard.SetActive(false);
        animator.SetBool("Skate", false);
        SkateTimer = 12f;
    }

    public float getCurrSpeed()
    {
        return currSpeed;
    }
}
