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
    public float speed = 7f;
    public float jumpForce = 15f;
    public float groundCheckRadius = 0.3f; // Increased for reliability

    [Header("Combat Settings")]
    public float attackRange = 2.0f; // Increased for ease of hit
    public LayerMask enemyLayer;
    public int damage = 1;
    public float comboResetTime = 1f;

    private float inputX;
    private bool isGrounded = false;
    private int jumpCounter = 0;
    private bool isAttacking = false;
    private int comboStep = 0;
    private float lastAttackTime;
    private float attackPointLocalX;

    void Awake()
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        if (boxCollider == null) boxCollider = GetComponent<BoxCollider2D>();
        if (animator == null) animator = GetComponent<Animator>();
        
        // Use manual bitmask if LayerMask.GetMask is being weird with duplicate names
        // Layer 3 is Enemy, Layer 7 is Ground, Layer 8 is Enemy
        groundLayer = (1 << 7); // Ground layer only
        enemyLayer = (1 << 3) | (1 << 8); // Both Enemy layers

        if (attackPoint == null)
        {
            var ap = transform.Find("AttackPoint");
            if (ap != null) attackPoint = ap;
            else
            {
                GameObject newAP = new GameObject("AttackPoint");
                newAP.transform.SetParent(transform);
                newAP.transform.localPosition = new Vector3(1.2f, 0, 0);
                attackPoint = newAP.transform;
            }
        }
        
        attackPointLocalX = Mathf.Abs(attackPoint.localPosition.x);
    }

    void Update()
    {
        if (Keyboard.current != null)
        {
            float targetX = 0;
            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) targetX = 1;
            else if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) targetX = -1;
            inputX = targetX;
        }

        CheckGrounded();

        if (Keyboard.current.spaceKey.wasPressedThisFrame)
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

        if (Mouse.current.leftButton.wasPressedThisFrame && !isAttacking)
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
        rb.linearVelocity = new Vector2(inputX * speed, rb.linearVelocity.y);
    }

    void CheckGrounded()
    {
        if (boxCollider == null) return;
        
        // Position at the feet
        Vector2 checkPos = (Vector2)transform.position + boxCollider.offset + Vector2.down * (boxCollider.size.y * transform.localScale.y * 0.5f);
        
        // Use OverlapCircle to detect ground tiles
        Collider2D hit = Physics2D.OverlapCircle(checkPos, groundCheckRadius, groundLayer);
        
        isGrounded = hit != null && hit.gameObject != gameObject;
    }

    void PerformJump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        isGrounded = false;
        if (animator != null) animator.SetBool("isGrounded", false);
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
        
        Debug.Log($"DealAttackDamage called at {attackPoint.position}. Range: {attackRange}, Mask: {enemyLayer.value}");
        
        // Use a filter to ensure we hit triggers too
        ContactFilter2D filter = new ContactFilter2D();
        filter.SetLayerMask(enemyLayer);
        filter.useTriggers = true;
        
        Collider2D[] hitEnemies = new Collider2D[10];
        int count = Physics2D.OverlapCircle(attackPoint.position, attackRange, filter, hitEnemies);
        
        Debug.Log($"Hit {count} colliders on enemy layer.");
        
        for(int i = 0; i < count; i++)
        {
            var enemy = hitEnemies[i];
            var health = enemy.GetComponent<EnemyHealth>();
            if (health == null) health = enemy.GetComponentInParent<EnemyHealth>();
            
            if (health != null)
            {
                health.TakeDamage(damage);
                Debug.Log("Damaged enemy: " + enemy.name);
            }
            else
            {
                Debug.Log("Hit object on enemy layer but found no EnemyHealth: " + enemy.name);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        if (boxCollider != null)
        {
            Gizmos.color = Color.green;
            Vector2 checkPos = (Vector2)transform.position + boxCollider.offset + Vector2.down * (boxCollider.size.y * transform.localScale.y * 0.5f);
            Gizmos.DrawWireSphere(checkPos, groundCheckRadius);
        }

        if (attackPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        }
    }
}