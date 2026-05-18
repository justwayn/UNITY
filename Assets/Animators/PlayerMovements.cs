using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovements : MonoBehaviour
{
    [Header("References")]
    public Rigidbody2D rb;
    public SpriteRenderer spriteRenderer;
    public BoxCollider2D boxCollider;
    public Animator animator;
    public LayerMask groundLayer;
    public Transform attackPoint;

    [Header("Movement Settings")]
    public float speed = 8f;
    public float jumpForce = 15f;
    public float groundCheckRadius = 0.3f;

    [Header("Dash Settings")]
    public float dashSpeed = 60f;
    public float dashDuration = 0.3f;
    public float dashCooldown = 0.5f;

    [Header("Combat Settings")]
    public float attackRange = 2.5f;
    public LayerMask enemyLayer;
    public int damage = 1;
    public float comboResetTime = 1f;

    private float inputX;
    private bool isGrounded = false;
    private int jumpCounter = 0;
    private bool isAttacking = false;
    private bool isDashing = false;
    private bool canDash = true;
    private int comboStep = 0;
    private float lastAttackTime;
    private float attackPointLocalX;

    void Awake()
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        if (boxCollider == null) boxCollider = GetComponent<BoxCollider2D>();
        if (animator == null) animator = GetComponent<Animator>();
        
        groundLayer = (1 << 7) | (1 << 9); 
        enemyLayer = (1 << 3) | (1 << 8); 

        if (attackPoint == null)
        {
            var ap = transform.Find("AttackPoint");
            if (ap != null) attackPoint = ap;
            else
            {
                GameObject newAP = new GameObject("AttackPoint");
                newAP.transform.SetParent(transform);
                newAP.transform.localPosition = new Vector3(1.5f, 0, 0);
                attackPoint = newAP.transform;
            }
        }
        
        attackPointLocalX = Mathf.Abs(attackPoint.localPosition.x);

        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            rb.gravityScale = 3f;
        }
    }

    void Update()
    {
        if (isDashing) return;

        if (Keyboard.current != null)
        {
            float targetX = 0;
            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) targetX = 1;
            else if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) targetX = -1;
            inputX = targetX;

            if ((Keyboard.current.leftShiftKey.wasPressedThisFrame || Keyboard.current.rightShiftKey.wasPressedThisFrame) && canDash)
            {
                int dashDir = inputX != 0 ? (int)Mathf.Sign(inputX) : (spriteRenderer.flipX ? -1 : 1);
                StartCoroutine(PerformDash(dashDir));
            }
        }

        CheckGrounded();

        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            if (isGrounded)
            {
                PerformJump();
                jumpCounter = 1; 
            }
            else if (jumpCounter > 0)
            {
                PerformJump();
                jumpCounter = 0;
            }
        }

        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame && !isAttacking)
        {
            StartAttack();
        }

        if (Time.time - lastAttackTime > comboResetTime)
        {
            comboStep = 0;
        }

        UpdateAnimator();

        if (inputX > 0.1f)
        {
            spriteRenderer.flipX = false;
            Vector3 pos = attackPoint.localPosition;
            pos.x = attackPointLocalX;
            attackPoint.localPosition = pos;
        }
        else if (inputX < -0.1f)
        {
            spriteRenderer.flipX = true;
            Vector3 pos = attackPoint.localPosition;
            pos.x = -attackPointLocalX;
            attackPoint.localPosition = pos;
        }
    }

    void FixedUpdate()
    {
        if (isDashing) return;
        rb.linearVelocity = new Vector2(inputX * speed, rb.linearVelocity.y);
    }

    void CheckGrounded()
    {
        Collider2D col = GetComponent<Collider2D>();
        if (col == null) return;
        
        Vector2 checkPos = new Vector2(col.bounds.center.x, col.bounds.min.y);
        Collider2D hit = Physics2D.OverlapCircle(checkPos, groundCheckRadius, groundLayer);
        
        bool wasGrounded = isGrounded;
        isGrounded = hit != null && hit.gameObject != gameObject;

        if (isGrounded && !wasGrounded)
        {
            jumpCounter = 1; 
        }
    }

    void PerformJump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        isGrounded = false;
        if (animator != null)
        {
            animator.SetBool("isGrounded", false);
            animator.SetBool("isJumping", true); 
        }
    }

    IEnumerator PerformDash(int direction)
    {
        canDash = false;
        isDashing = true;

        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;
        
        RigidbodyConstraints2D originalConstraints = rb.constraints;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionY;

        Vector2 dashVelocity = new Vector2(direction * dashSpeed, 0f);
        
        if (animator != null)
        {
            animator.SetBool("isDodging", true);
            animator.SetTrigger("Dash");
        }

        float elapsed = 0;
        while (elapsed < dashDuration)
        {
            rb.linearVelocity = dashVelocity;
            elapsed += Time.deltaTime;
            yield return null;
        }

        if (animator != null) animator.SetBool("isDodging", false);

        rb.gravityScale = originalGravity;
        rb.constraints = originalConstraints;
        rb.linearVelocity = Vector2.zero;
        isDashing = false;

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    void StartAttack()
    {
        isAttacking = true;
        lastAttackTime = Time.time;

        if (animator != null)
        {
            animator.SetInteger("AttackNum", comboStep);
            animator.SetTrigger("isAttacking");
        }

        comboStep = (comboStep + 1) % 3;
        StartCoroutine(ResetAttack());
    }

    IEnumerator ResetAttack()
    {
        yield return new WaitForSeconds(0.4f);
        isAttacking = false;
    }

    void UpdateAnimator()
    {
        if (animator == null) return;

        animator.SetBool("isGrounded", isGrounded);
        animator.SetBool("isRunning", Mathf.Abs(inputX) > 0.1f);
        animator.SetFloat("speed", Mathf.Abs(rb.linearVelocity.x));
        animator.SetBool("isJumping", !isGrounded);
    }

    public void DealAttackDamage()
    {
        if (attackPoint == null) return;
        
        ContactFilter2D filter = new ContactFilter2D();
        filter.SetLayerMask(enemyLayer);
        filter.useTriggers = true;
        
        Collider2D[] hitEnemies = new Collider2D[10];
        int count = Physics2D.OverlapCircle(attackPoint.position, attackRange, filter, hitEnemies);
        
        for(int i = 0; i < count; i++)
        {
            var enemy = hitEnemies[i];
            
            var health = enemy.GetComponent<EnemyHealth>();
            if (health == null) health = enemy.GetComponentInParent<EnemyHealth>();
            if (health != null)
            {
                health.TakeDamage(damage);
                continue;
            }

            var ce = enemy.GetComponentInParent<CrystalEnemyAI>();
            if (ce != null)
            {
                ce.TakeDamage(damage);
                continue;
            }

            var tb = enemy.GetComponentInParent<TentacleBossAI>();
            if (tb != null)
            {
                tb.TakeDamage(damage);
                continue;
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        }
    }
}