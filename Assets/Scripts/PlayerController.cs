using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 5f;
    public float jumpForce = 7f;

    [Header("Jump")]
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

    private float moveInput;
    private int jumpCount;

    private bool isGrounded;
    private bool isFacingRight = true;

    private bool isDashing;
    private bool canDash = true;

    private float lastTapA;
    private float lastTapD;
    private float attackPointStartX;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        if (attackPoint != null)
            attackPointStartX = Mathf.Abs(attackPoint.localPosition.x);

        // Important: make sure player is not frozen
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.simulated = true;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    void Update()
    {
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
            Flip();

        if (direction < 0 && isFacingRight)
            Flip();

        if (animator != null)
            animator.SetTrigger("Dash");

        rb.linearVelocity = new Vector2(direction * dashSpeed, rb.linearVelocity.y);

        yield return new WaitForSeconds(dashDuration);

        isDashing = false;

        yield return new WaitForSeconds(dashCooldown);

        canDash = true;
    }

    void HandleFlip()
    {
        if (moveInput > 0 && !isFacingRight)
            Flip();

        if (moveInput < 0 && isFacingRight)
            Flip();
    }

    void Flip()
    {
        isFacingRight = !isFacingRight;

        if (spriteRenderer != null)
            spriteRenderer.flipX = !isFacingRight;

        if (attackPoint != null)
        {
            Vector3 pos = attackPoint.localPosition;

            if (isFacingRight)
                pos.x = attackPointStartX;
            else
                pos.x = -attackPointStartX;

            attackPoint.localPosition = pos;
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