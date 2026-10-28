using UnityEngine;
using UnityEngine.InputSystem; // Importante incluir esta librería

public class SimplePlayerMovement : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    public float speed = 3f;
    public bool autoMove = false; // Actívalo en el Inspector si quieres que camine solo

    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer sprite;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        float horizontal = 0f;

        if (autoMove)
        {
            horizontal = 1f;
        }
        else
        {
            // Lectura con el nuevo Input System
            if (Keyboard.current != null)
            {
                if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
                {
                    horizontal = 1f;
                }
                else if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
                {
                    horizontal = -1f;
                }
            }
        }

        // Mover Rigidbody
        rb.linearVelocity = new Vector2(horizontal * speed, rb.linearVelocity.y);

        // Voltear Sprite
        if (horizontal > 0) sprite.flipX = false;
        else if (horizontal < 0) sprite.flipX = true;

        // Animación
        if (anim != null)
        {
            bool isMoving = Mathf.Abs(horizontal) > 0.01f;
            anim.SetBool("isWalking", isMoving);
        }
    }
}