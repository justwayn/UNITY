using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 8f;
    public float jumpForce = 12f;

    [Header("Jump")]
    public int maxJumps = 2;

    [Header("Dash")]
    public float dashSpeed = 40f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 0.5f;

    [Header("Flip Settings")]
    public SpriteRenderer spriteRenderer;
    public Transform attackPoint;

    private Rigidbody2D rb;
    private Animator animator;

    private float moveInput;
    private int jumpCount;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    private bool isGrounded;
    private bool isFacingRight = true;

    private bool isDashing;
    private bool canDash = true;

    private float attackPointStartX;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        if (speed <= 0) speed = 5f;
        if (dashSpeed <= 0) dashSpeed = 20f;
        if (dashDuration <= 0) dashDuration = 0.2f;

        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        if (attackPoint != null)
            attackPointStartX = Mathf.Abs(attackPoint.localPosition.x);

        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.simulated = true;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate; // SMOOTH CAMERA FOLLOW
        }

    void Update()
    {
        // Don't process input if dead or game over
        if (GameManager.Instance != null && GameManager.Instance.isGameOver) return;

        // Reliable Ground Check using OverlapCircle
        bool wasGrounded = isGrounded;
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        
        if (isGrounded && !wasGrounded)
        {
            jumpCount = 0;
        }

        moveInput = Input.GetAxisRaw("Horizontal");

        HandleFlip();

        if (animator != null)
        {
            animator.SetFloat("speed", Mathf.Abs(moveInput));
            animator.SetBool("isJumping", !isGrounded);
            animator.SetBool("isGrounded", isGrounded);
            animator.SetBool("isRunning", Mathf.Abs(moveInput) > 0.1f && isGrounded);
        }

        HandleDash();

        if (Input.GetKeyDown(KeyCode.Space) && jumpCount < maxJumps)
        {
            Jump();
        }
    }

    void FixedUpdate()
    {
        if (isDashing || (GameManager.Instance != null && GameManager.Instance.isGameOver))
        {
             if (isDashing) return;
             rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
             return;
        }

        rb.linearVelocity = new Vector2(moveInput * speed, rb.linearVelocity.y);
    }

    void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        jumpCount++;
    }

    void HandleDash()
    {
        if (!canDash || (GameManager.Instance != null && GameManager.Instance.isGameOver)) return;

        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
        {
            int direction = isFacingRight ? 1 : -1;
            if (moveInput > 0) direction = 1;
            else if (moveInput < 0) direction = -1;

            StartCoroutine(Dash(direction));
        }
    }

    IEnumerator Dash(int direction)
    {
        canDash = false;
        isDashing = true;

        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;
        
        // Use a fixed force/velocity for the dash
        // We set high velocity and maintain it throughout the duration
        Vector2 dashVelocity = new Vector2(direction * dashSpeed, 0f);
        
        if (animator != null)
        {
            animator.SetBool("isDodging", true);
            animator.SetTrigger("Dash");
        }

        float elapsed = 0;
        while (elapsed < dashDuration)
        {
            rb.linearVelocity = dashVelocity; // Force velocity every frame
            elapsed += Time.deltaTime;
            yield return null;
        }

        if (animator != null)
        {
            animator.SetBool("isDodging", false);
        }

        rb.gravityScale = originalGravity;
        rb.linearVelocity = new Vector2(0, 0); // Complete stop
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