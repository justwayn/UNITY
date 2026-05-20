using UnityEngine;
using System.Collections;

public class SoldierAI : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 2f;
    public float chaseSpeed = 4f;
    public float detectionRange = 10f;
    public float patrolRange = 5f;
    public float movementSmoothing = 0.1f;
    private Vector3 startPosition;
    private Vector3 patrolTarget;
    private Transform player;
    private float currentVelocityX;

    [Header("Attack Settings")]
    public float attackRange = 1.5f;
    public float attackCooldown = 2f;
    public int attackDamage = 1;
    public Transform attackPoint;
    public float attackCircleRadius = 1f;
    public LayerMask playerLayer;
    public bool canMoveDuringAttack = false;
    private float lastAttackTime;

    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private EnemyHealth enemyHealth;

    void Start()
    {
        startPosition = transform.position;
        SetNewPatrolTarget();
        
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        enemyHealth = GetComponent<EnemyHealth>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;
    }

    void Update()
    {
        if (enemyHealth != null && IsDead()) return;

        float distanceToPlayer = player != null ? Vector2.Distance(transform.position, player.position) : float.MaxValue;

        // Determine behavior
        if (distanceToPlayer <= attackRange)
        {
            Attack();
        }
        else if (distanceToPlayer <= detectionRange)
        {
            ChasePlayer();
        }
        else
        {
            Patrol();
        }

        UpdateAnimation();
    }

    bool IsDead()
    {
        return !enabled;
    }

    void Patrol()
    {
        MoveTo(patrolTarget, moveSpeed);
        if (Vector2.Distance(transform.position, patrolTarget) < 0.2f)
        {
            SetNewPatrolTarget();
        }
    }

    void SetNewPatrolTarget()
    {
        float randomX = Random.Range(-patrolRange, patrolRange);
        patrolTarget = new Vector3(startPosition.x + randomX, transform.position.y, transform.position.z);
    }

    void ChasePlayer()
    {
        if (player == null) return;
        Vector3 targetPos = new Vector3(player.position.x, transform.position.y, transform.position.z);
        MoveTo(targetPos, chaseSpeed);
    }

    void MoveTo(Vector3 target, float speed)
    {
        float targetVelocityX = (target.x > transform.position.x) ? speed : -speed;
        
        // Smoother Velocity Transition
        float newVelocityX = Mathf.SmoothDamp(rb.linearVelocity.x, targetVelocityX, ref currentVelocityX, movementSmoothing);
        rb.linearVelocity = new Vector2(newVelocityX, rb.linearVelocity.y);

        // Face player/movement direction smoothly
        UpdateFacingDirection();
    }

    void UpdateFacingDirection()
    {
        if (player != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);
            if (distanceToPlayer <= detectionRange)
            {
                // Direct flip to prevent squishing and ensure responsiveness
                spriteRenderer.flipX = player.position.x < transform.position.x;
            }
            else if (Mathf.Abs(rb.linearVelocity.x) > 0.1f)
            {
                spriteRenderer.flipX = rb.linearVelocity.x < 0;
            }
        }
        else if (Mathf.Abs(rb.linearVelocity.x) > 0.1f)
        {
            spriteRenderer.flipX = rb.linearVelocity.x < 0;
        }

        UpdateAttackPointFlip();
    }

    void UpdateAttackPointFlip()
    {
        if (attackPoint != null)
        {
            Vector3 pos = attackPoint.localPosition;
            pos.x = spriteRenderer.flipX ? -Mathf.Abs(pos.x) : Mathf.Abs(pos.x);
            attackPoint.localPosition = pos;
        }
    }

    void Attack()
    {
        // Smoothly come to a stop during attack
        float newVelocityX = Mathf.SmoothDamp(rb.linearVelocity.x, 0, ref currentVelocityX, movementSmoothing);
        rb.linearVelocity = new Vector2(newVelocityX, rb.linearVelocity.y);

        // Ensure we are facing the player accurately during the attack wind-up
        UpdateFacingDirection();

        if (Time.time >= lastAttackTime + attackCooldown)
        {
            animator.SetTrigger("Attack");
            lastAttackTime = Time.time;
        }
    }

    public void DealDamage()
    {
        if (attackPoint == null) return;

        Collider2D hit = Physics2D.OverlapCircle(attackPoint.position, attackCircleRadius, playerLayer);
        if (hit != null)
        {
            PlayerHealth playerHealth = hit.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(attackDamage);
            }
        }
    }

    void UpdateAnimation()
    {
        animator.SetFloat("Speed", Mathf.Abs(rb.linearVelocity.x));
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        if (attackPoint != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(attackPoint.position, attackCircleRadius);
        }
    }
}


