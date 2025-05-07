using System;
using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;
    public CameraFollowOffset cam;
    public Particles particles;

    [Header("Movement Settings")] public float moveSpeed = 10f;
    public float moveForwardSpeed = 10f;
    public float BoostSpeed = 10f;
    public float jumpForce = 8f;
    public float rotationSpeed = 5f;
    public float maxRotation = 15f;
    public float GroundCheckRayCastLenght = 1f;
    public LayerMask GroundLayer;

    [Header("Slide Settings")] public float slideDuration = 0.5f;
    private float slideTimer = 0f;

    [Header("Boundaries")] public float minX = -5f;
    public float maxX = 5f;

    [Header("Animation Settings")] public Animator animator;

    [Header("Boost Management")] public bool BoostEnabled = false;
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
    public bool isJumping = false;
    public bool isSliding = false;
    public bool canMovement = true;
    public bool isHurt = false;
    public bool isStumble = false;
    bool wasGroundedLastFrame = false;

    [Header("Start mach")] public int StartingCountDown;
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
        Audiomanager.instance.audi_bg.mute = true;

        Audiomanager.instance.audi_bg.clip = Audiomanager.instance.Bg_2;
        Audiomanager.instance.audi_bg.Play();

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
        Audiomanager.instance.PlayCountDown_Time(); // Play once per second

        while (currentTime > 0)
        {
            if (StartingCountDownText != null)
                StartingCountDownText.text = Mathf.Ceil(currentTime).ToString("0");

            yield return new WaitForSeconds(1f); // Wait for 1 second
            Audiomanager.instance.PlayCountDown_Time(); // Play once per second

            currentTime--;
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
            transform.rotation =
                Quaternion.Lerp(transform.rotation, Quaternion.identity, Time.deltaTime * rotationSpeed);
        }
    }

    void HandleJumpAndSlide()
    {
        bool currentlyGrounded = IsGrounded();
        isGrounded = currentlyGrounded;


        // Landing sound
        if (!wasGroundedLastFrame && currentlyGrounded && IsSkateBoardOn)
        {
            Audiomanager.instance.Play_SkateLand();
            Audiomanager.instance.SkateBoard_Source.mute = false;
            DOTween.To(() => Audiomanager.instance.SkateBoard_Source.volume,
                x => Audiomanager.instance.SkateBoard_Source.volume = x, 0.5f, 1f);
        }

        // Reset jump animation
        if (currentlyGrounded && animator.GetBool("Jump"))
        {
            animator.SetBool("Jump", false);
        }

        // Disable movement while airborne or disallowed
        if (!canMovement || !isGrounded)
        {
            wasGroundedLastFrame = isGrounded;
            return;
        }

        // Handle jump
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            isJumping = true;

            if (IsSkateBoardOn)
            {
                Audiomanager.instance.Play_SkateJump();
                Audiomanager.instance.SkateBoard_Source.mute = true;
                DOTween.To(() => Audiomanager.instance.SkateBoard_Source.volume,
                    x => Audiomanager.instance.SkateBoard_Source.volume = x, 0f, 1f);
            }
            else
            {
                Audiomanager.instance.PlayJump_Sfx();
            }

            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            
            animator.SetBool("Slide", false);
            animator.SetBool("Jump", true);
            
            PlayerCollider.center = new Vector3(0f, 0.9869743f, 0.1132071f);
            PlayerCollider.size = new Vector3(1, 1.979002f, 1.00319f);
        }

        // Handle slide
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            StartSlide();
        }

        wasGroundedLastFrame = isGrounded;
    }
    public void ToggleMagnet()
    {
        IsMagnetOn = !IsMagnetOn;

        if (IsMagnetOn)
        {
            CashCollider.size = new Vector3(100, 50, 1);
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
        Audiomanager.instance.PlaySlide_Sfx();
        PlayerCollider.center = new Vector3(0f, 0.2507838f, 0.1132071f);
        PlayerCollider.size = new Vector3(1, 0.5066212f, 1.00319f);
        isSliding = true;
        slideTimer = slideDuration;
        if (IsSkateBoardOn)
        {
            animator.SetTrigger("SkateSlide");
        }
        else
        {
            animator.SetBool("Slide",true);
        }
    }

    public void EndSlide() // Called from animation event
    {
        isSliding = false;
        PlayerCollider.center = new Vector3(0f, 0.9869743f, 0.1132071f);
        PlayerCollider.size = new Vector3(1, 1.979002f, 1.00319f);
        animator.SetBool("Slide",false);
        animator.SetTrigger("Run");
    }

    private bool IsGrounded()
    {
        Vector3 origin = transform.position + Vector3.up * 0.1f;
        animator.SetBool("Jump", false);
        return Physics.Raycast(origin, Vector3.down, GroundCheckRayCastLenght, GroundLayer);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {
            Hurt();
            if (other.gameObject.GetComponent<Obstacle>() != null)
            {
                other.gameObject.GetComponent<Obstacle>().ToggleCarOnHit();
            }
        }
        else if (other.CompareTag("Collectable"))
        {
            Collect(other.gameObject.GetComponent<Collectable>().myid);
        }
    }

    private void Hurt()
    {
        if (isHurt || isStumble)
            return;

        particles.HitEffect.Play();
        cam.Shake();

        if (BoostEnabled)
        {
            DOTween.To(() => Audiomanager.instance.Player_Source.volume,
                x => Audiomanager.instance.Player_Source.volume = x, 0f, 0.3f);
        }

        if (IsSkateBoardOn)
        {
            Audiomanager.instance.SkateBoard_Source.mute = true;
            DOTween.To(() => Audiomanager.instance.SkateBoard_Source.volume,
                x => Audiomanager.instance.SkateBoard_Source.volume = x, 0f, 1f);
        }

        isHurt = true;
        
        animator.SetBool("Slide",false);
        
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
        particles.CoinPick.Play();
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
            moveForwardSpeed = BoostSpeed;
            BoostTimer -= Time.deltaTime;
            if (BoostTimer <= 0)
            {
                BoostEnabled = false;
                
                particles.SpeedLines.gameObject.SetActive(false);

                moveForwardSpeed = 25f;

                DOTween.To(() => Audiomanager.instance.Player_Source.volume,
                    x => Audiomanager.instance.Player_Source.volume = x, 0f, 0.3f);

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
        Audiomanager.instance.audi_bg.mute = false;
        Audiomanager.instance.audi_bg.time = 0;
        animator.SetBool("Transit", true);
        moveForwardSpeed = 25f;
        GameManager.Instance.gameEnded = false;
        canMovement = true;
        GameManager.Instance.progressBar.transform.parent.gameObject.SetActive(true);
        GameManager.Instance.TimerText.gameObject.SetActive(true);
    }

    public void RestHurt() // Called in animation event
    {
        isHurt = false;
        canMovement = true;
        
        if (IsSkateBoardOn)
        {
            PlayerCollider.center = new Vector3(0f, 0.8697391f, 0.1132071f);
            PlayerCollider.size = new Vector3(1, 2.213472f, 1.00319f);
            Audiomanager.instance.SkateBoard_Source.mute = false;
            DOTween.To(() => Audiomanager.instance.SkateBoard_Source.volume,
                x => Audiomanager.instance.SkateBoard_Source.volume = x, 0.5f, 1f);
        }
        else
        {
            PlayerCollider.center = new Vector3(0f, 0.9869743f, 0.1132071f);
            PlayerCollider.size = new Vector3(1, 1.979002f, 1.00319f);
        }

        if (BoostEnabled)
        {
            DOTween.To(() => Audiomanager.instance.Player_Source.volume,
                x => Audiomanager.instance.Player_Source.volume = x, 0.5f, 0.7f);
        }

        StopAllEnvironment(true);
    }

    public void ResetStumble()
    {
        if (IsSkateBoardOn)
        {
            Audiomanager.instance.SkateBoard_Source.mute = false;
            DOTween.To(() => Audiomanager.instance.SkateBoard_Source.volume,
                x => Audiomanager.instance.SkateBoard_Source.volume = x, 0.5f, 1f);
        }

        isStumble = false;
        canMovement = true;
        moveForwardSpeed = 25;
        moveSpeed = 7f;
        PlayerCollider.center = new Vector3(0f, 0.9869743f, 0.1132071f);
        PlayerCollider.size = new Vector3(1, 1.979002f, 1.00319f);
    }

    void StopSkate()
    {
        if (BoostEnabled)
        {
            Audiomanager.instance.Player_Source.mute = false;
            DOTween.To(() => Audiomanager.instance.Player_Source.volume,
                x => Audiomanager.instance.Player_Source.volume = x, 0.5f, 1f);
        }

        particles.SpeedLines.gameObject.SetActive(false);
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

[Serializable]
public class Particles
{
    public ParticleSystem CoinPick;
    public ParticleSystem HitEffect;
    public ParticleSystem Stumble;
    public ParticleSystem SkatePick;
    public ParticleSystem SpeedLines;
}