using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpForce = 8f;
    [SerializeField] private Transform RespawnPoint;

    [SerializeField] private ScreenFade screenFade;
    private CameraFocus cameraFocus;
    private bool isTeleporting = false;

    [Header("Ground check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.15f;
    [SerializeField] private LayerMask groundLayer;

    private SpriteRenderer sr;
    private Animator animator;

    private GameManager gameManager;
    private Vector2 moveInput;
    private bool jumpRequested;
    private bool isGrounded;
    private PlayerInput playerInput;
    private Rigidbody2D rb;

    

    [Header("Shadow")]
    [SerializeField] private GameObject shadowPrefab;
    [SerializeField] private float recallDuration = 0.5f;

    [SerializeField] private float swapDuration = 0.4f;
    private bool isSwapping = false;
    public GameObject activeShadow;
    public bool hasControl = true;
    private bool isRecalling = false;


    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        gameManager = FindAnyObjectByType<GameManager>();
        cameraFocus = FindAnyObjectByType<CameraFocus>();
        animator = GetComponent<Animator>();
        playerInput = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody2D>();
    }

    

    void Update()
    {
        if (transform.position.y < -10 && !isTeleporting)
        {
            StartCoroutine(TeleportTransition(
                afterFade: () => gameManager.SetDialogo("Es broma, el limbo, ¿recuerdas?")
            ));
        }

        if (!hasControl && activeShadow == null) hasControl = true;

        if (Time.timeScale == 0)
        {
            moveInput = Vector2.zero;
            return;
        }

        if (playerInput.actions["Duplicate"].WasPressedThisFrame() && !isRecalling && !isSwapping)
        {
            if (activeShadow == null) SpawnShadow();
            else StartCoroutine(RecallShadow());
        }

        if (playerInput.actions["Change"].WasPressedThisFrame() && activeShadow != null && !isRecalling && !isSwapping)
        {
           StartCoroutine(SwapWithShadow());
        }

        if (!hasControl)
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

    IEnumerator SwapWithShadow()
    {
        isSwapping = true;

        Transform shadow = activeShadow.transform;
        Rigidbody2D shadowRb = activeShadow.GetComponent<Rigidbody2D>();
        ShadowController shadowCtrl = activeShadow.GetComponent<ShadowController>();

        // Congela control y física de ambos mientras dura la animación
        shadowCtrl.enabled = false;
        shadowRb.simulated = false;
        rb.simulated = false;

        Vector3 playerStart = transform.position;
        Vector3 shadowStart = shadow.position;
        Vector3 mid = (playerStart + shadowStart) / 2f;
        Vector3 playerScale = transform.localScale;
        Vector3 shadowScale = shadow.localScale;
        float half = swapDuration / 2f;

        // Fase 1: ambos se encogen hacia el punto medio
        for (float e = 0f; e < half; e += Time.deltaTime)
        {
            float t = e / half;
            float eased = t * t;   // arranca lento y acelera

            transform.position = Vector3.Lerp(playerStart, mid, eased);
            shadow.position = Vector3.Lerp(shadowStart, mid, eased);
            transform.localScale = Vector3.Lerp(playerScale, Vector3.zero, eased);
            shadow.localScale = Vector3.Lerp(shadowScale, Vector3.zero, eased);

            yield return null;
        }

        // Fase 2: crecen yendo cada uno a la posición del otro
        for (float e = 0f; e < half; e += Time.deltaTime)
        {
            float t = e / half;
            float eased = 1f - (1f - t) * (1f - t);   // arranca rápido y frena

            transform.position = Vector3.Lerp(mid, shadowStart, eased);
            shadow.position = Vector3.Lerp(mid, playerStart, eased);
            transform.localScale = Vector3.Lerp(Vector3.zero, playerScale, eased);
            shadow.localScale = Vector3.Lerp(Vector3.zero, shadowScale, eased);

            yield return null;
        }

        // Valores exactos al terminar, para que no queden errores de redondeo
        transform.position = shadowStart;
        shadow.position = playerStart;
        transform.localScale = playerScale;
        shadow.localScale = shadowScale;

        // Reactiva la física y limpia la velocidad
        rb.simulated = true;
        shadowRb.simulated = true;
        rb.linearVelocity = Vector2.zero;
        shadowRb.linearVelocity = Vector2.zero;
        shadowCtrl.enabled = true;

        isSwapping = false;
    }

    void SpawnShadow()
    {
        activeShadow = Instantiate(shadowPrefab, transform.position, Quaternion.identity);
        activeShadow.GetComponent<ShadowController>().Init(playerInput);
        hasControl = false;
        moveInput = Vector2.zero;
        jumpRequested = false;
    }

    public void RecallS()
    {
        StartCoroutine(RecallShadow());
    }
    IEnumerator RecallShadow()
    {
        isRecalling = true;

        Transform shadow = activeShadow.transform;

        
        activeShadow.GetComponent<ShadowController>().enabled = false;
        activeShadow.GetComponent<Rigidbody2D>().simulated = false;

        Vector3 startPos = shadow.position;
        Vector3 startScale = shadow.localScale;
        float elapsed = 0f;

        while (elapsed < recallDuration && shadow != null)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / recallDuration);
            float eased = t * t;  

            shadow.position = Vector3.Lerp(startPos, transform.position, eased);
            shadow.localScale = Vector3.Lerp(startScale, Vector3.zero, eased);

            yield return null;
        }

        if (activeShadow != null) Destroy(activeShadow);
        activeShadow = null;
        hasControl = true;
        isRecalling = false;
    }

    void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        float targetX = Mathf.Abs(moveInput.x) > 0.2f ? moveInput.x * speed : 0f;
        rb.linearVelocity = new Vector2(targetX, rb.linearVelocity.y);

        if (jumpRequested && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            animator.SetTrigger("Jump");
        }
        jumpRequested = false;

        animator.SetBool("IsGrounded", isGrounded);
        animator.SetFloat("VelocityY", rb.linearVelocity.y);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Key"))
        {
            gameManager.AddKey(collision.gameObject);
            LoopManager lp = FindAnyObjectByType<LoopManager>();
            lp.keysGot++;
            Destroy(collision.gameObject);
        }

        if (collision.CompareTag("Door"))
        {
            if(gameManager.TryUnlockDoor(collision.gameObject))
                Destroy(collision.gameObject);
        }
        if (collision.CompareTag("Puzzle"))
        {
            Puzzle pz = collision.GetComponent<Puzzle>();
            pz.ActivateMechanism();
            Animator animator = collision.GetComponent<Animator>();
            if (animator.GetBool("Activada"))
            {
                animator.SetBool("Activada", false);
            }
            else
            {
                animator.SetBool("Activada", true);
            }
        }
        if (collision.CompareTag("Button"))
        {
            collision.GetComponent<Puzzle>().Press(gameObject);
            if(collision.GetComponent<Animator>() != null)
                collision.GetComponent<Animator>().SetBool("Pulsado", true);
        }
        if (collision.CompareTag("Portal"))
        {
            if (collision.CompareTag("Portal") && !isTeleporting)
            {
                LoopManager loopManager = FindAnyObjectByType<LoopManager>();
                StartCoroutine(TeleportTransition(atPeak: () => loopManager.RestartLoop()));
            }

        }
    }

    IEnumerator TeleportTransition(System.Action atPeak = null, System.Action afterFade = null)
    {
        isTeleporting = true;
        moveInput = Vector2.zero;
        rb.linearVelocity = Vector2.zero;
        rb.simulated = false;

        yield return screenFade.Transition(() =>
        {
            if (activeShadow != null)
            {
                Destroy(activeShadow);
                activeShadow = null;
                hasControl = true;
            }

            transform.position = RespawnPoint.position;
            if (cameraFocus != null) cameraFocus.SnapToTarget();
            atPeak?.Invoke();
        });

        rb.simulated = true;
        isTeleporting = false;
        afterFade?.Invoke();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Button"))
        {
            collision.GetComponent<Puzzle>().Release(gameObject);
            if (collision.GetComponent<Animator>() != null)
                collision.GetComponent<Animator>().SetBool("Pulsado", false);
        }
    }
}