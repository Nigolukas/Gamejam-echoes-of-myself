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

    private PlayerInput playerInput;
    private Rigidbody2D rb;
    private Vector2 moveInput;
    private bool jumpRequested;
    private bool isGrounded;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Init(PlayerInput input)
    {
        playerInput = input;
    }

    void Update()
    {
        if (playerInput == null || Time.timeScale == 0)
        {
            moveInput = Vector2.zero;
            return;
        }

        moveInput = playerInput.actions["Move"].ReadValue<Vector2>();

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
    }
}