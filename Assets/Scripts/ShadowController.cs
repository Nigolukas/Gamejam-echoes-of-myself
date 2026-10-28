using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShadowController : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpForce = 8f;

    [Header("Ground check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.15f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask playerLayer;
    private SpriteRenderer sr;
    private Animator animator;
    private PlayerInput playerInput;
    private Rigidbody2D rb;
    private Vector2 moveInput;
    private bool jumpRequested;
    private bool isGrounded;

    

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    public void Init(PlayerInput input)
    {
        playerInput = input;
    }

    void Update()
    {
        if (transform.position.y < -10 )
        {
            PlayerController playerC = FindAnyObjectByType<PlayerController>();
            playerC.RecallS(); 
        }
            if (playerInput == null || Time.timeScale == 0)
        {
            moveInput = Vector2.zero;
            return;
        }

        moveInput = playerInput.actions["Move"].ReadValue<Vector2>();
        animator.SetFloat("SpeedX", Mathf.Abs(moveInput.x));

        if (moveInput.x > 0.2f) sr.flipX = false;
        else if (moveInput.x < -0.2f) sr.flipX = true;

        if (playerInput.actions["Jump"].WasPressedThisFrame() || moveInput.y > 0.2f)
        {
            jumpRequested = true;
        }

    }

    void FixedUpdate()
    {

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer | playerLayer);

        float targetX = Mathf.Abs(moveInput.x) > 0.2f ? moveInput.x * speed : 0f;
        rb.linearVelocity = new Vector2(targetX, rb.linearVelocity.y);

        if (jumpRequested && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
        jumpRequested = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Puzzle"))
        {
            Puzzle pz = collision.GetComponent<Puzzle>();
            pz.ActivateMechanism();
        }
        if (collision.CompareTag("Button"))
        {
            collision.GetComponent<Puzzle>().Press(gameObject);
            if(collision.GetComponent<Animator>() != null)
                collision.GetComponent<Animator>().SetTrigger("Pulsar");
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Button"))
        {
            collision.GetComponent<Puzzle>().Release(gameObject);
        }
    }
}