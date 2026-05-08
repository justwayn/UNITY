using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 5f;
    public float jumpForce = 7f;

    [Header("Jump Settings")]
    public int maxJumps = 2;

    [Header("Dash")]
    public float dashSpeed = 14f;
    public float dashDuration = 0.15f;
    public float dashCooldown = 0.5f;
    public float doubleTapTime = 0.25f;

    [Header("Flip Settings")]
    public SpriteRenderer spriteRenderer;
    public Transform attackPoint;

    private Rigidbody2D rb;
    private Animator animator;

    private bool isGrounded;
    private bool isDashing;
    private bool canDash = true;

    private float moveInput;
    private bool isFacingRight = true;
    private float attackPointStartX;

    private float lastTapA;
    private float lastTapD;

    private int jumpCount;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        if (attackPoint != null)
        {
            attackPointStartX = Mathf.Abs(attackPoint.localPosition.x);
        }
    }

    void Update()
    {
        if (isDashing) return;

        moveInput = Input.GetAxisRaw("Horizontal");

        HandleFlip();

        if (animator != null)
        {
            animator.SetFloat("speed", Mathf.Abs(moveInput));
            animator.SetBool("isJumping", !isGrounded);
        }

        HandleDoubleTapDash();

        if (Input.GetKeyDown(KeyCode.Space) && jumpCount < maxJumps)
        {
            Jump();
        }
    }

    void FixedUpdate()
    {
        if (isDashing) return;

        Move();
    }

    void Move()
    {
        rb.linearVelocity = new Vector2(moveInput * speed, rb.linearVelocity.y);
    }

    void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);

        jumpCount++;
        isGrounded = false;
    }

    void HandleDoubleTapDash()
    {
        if (!canDash) return;

        if (Input.GetKeyDown(KeyCode.D))
        {
            if (Time.time - lastTapD <= doubleTapTime)
            {
                StartCoroutine(Dash(1));
            }

            lastTapD = Time.time;
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            if (Time.time - lastTapA <= doubleTapTime)
            {
                StartCoroutine(Dash(-1));
            }

            lastTapA = Time.time;
        }
    }

    IEnumerator Dash(int direction)
    {
        canDash = false;
        isDashing = true;

        if (direction > 0 && !isFacingRight)
        {
            Flip();
        }
        else if (direction < 0 && isFacingRight)
        {
            Flip();
        }

        if (animator != null)
        {
            animator.SetTrigger("Dash");
        }

        rb.linearVelocity = new Vector2(direction * dashSpeed, 0f);

        yield return new WaitForSeconds(dashDuration);

        isDashing = false;

        yield return new WaitForSeconds(dashCooldown);

        canDash = true;
    }

    void HandleFlip()
    {
        if (moveInput > 0 && !isFacingRight)
        {
            Flip();
        }
        else if (moveInput < 0 && isFacingRight)
        {
            Flip();
        }
    }

    void Flip()
    {
        isFacingRight = !isFacingRight;

        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = !isFacingRight;
        }

        if (attackPoint != null)
        {
            Vector3 newAttackPointPosition = attackPoint.localPosition;

            if (isFacingRight)
            {
                newAttackPointPosition.x = attackPointStartX;
            }
            else
            {
                newAttackPointPosition.x = -attackPointStartX;
            }

            attackPoint.localPosition = newAttackPointPosition;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            jumpCount = 0;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }
}