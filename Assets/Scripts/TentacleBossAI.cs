using UnityEngine;
using System.Collections;

public class TentacleBossAI : MonoBehaviour
{
    [Header("Health")]
    public int health = 5;
    private int maxHealth;

    [Header("Movement")]
    public float moveSpeed = 2f;
    public float followSmoothing = 0.1f;
    private float currentVelocityX;

    [Header("Combat Settings")]
    public float detectionRange = 12f;
    public float attackRange = 8f;
    public float attackCooldown = 2f;
    public float phase1ExposeTime = 3f;
    public float phase2ExposeTime = 1.5f;
    public float attackInterval = 1f;
    public float attackIntervalPhase2 = 0.5f;

    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Collider2D bossCollider;
    private Collider2D tipCollider;
    private Transform player;
    private bool isExposed = false;
    private bool isDead = false;
    private float lastAttackTime;

    public enum BossPhase { Phase1, Phase2, Frenzy }
    public BossPhase currentPhase = BossPhase.Phase1;

    void Start()
    {
        maxHealth = health;
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        bossCollider = GetComponent<Collider2D>();
        
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.simulated = true;
        }

        if (bossCollider != null) bossCollider.isTrigger = true;

        Transform tip = transform.Find("TentacleTip");
        if (tip != null)
        {
            tipCollider = tip.GetComponent<Collider2D>();
            tipCollider.enabled = false;
        }

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;
    }

    [Header("Attack Settings")]
    public Transform attackPoint;
    public float attackCircleRadius = 2f;
    public LayerMask playerLayer;

    void Update()
    {
        if (isDead) return;

        // CRITICAL: Check if all crystals are destroyed
        if (Object.FindObjectsByType<CrystalEnemyAI>(FindObjectsSortMode.None).Length == 0)
        {
            Die();
            return;
        }

        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) player = playerObj.transform;
            return;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // STATIONARY: Tentacles stay in their position.
        // Direct flip to ensure no "squishing" while tracking the player.
        Vector3 currentScale = transform.localScale;
        if (player.position.x > transform.position.x)
        {
            currentScale.x = Mathf.Abs(currentScale.x);
        }
        else
        {
            currentScale.x = -Mathf.Abs(currentScale.x);
        }
        transform.localScale = currentScale;

        // Sync attack point flip
        if (attackPoint != null)
        {
            Vector3 apPos = attackPoint.localPosition;
            apPos.x = (transform.localScale.x > 0) ? Mathf.Abs(apPos.x) : -Mathf.Abs(apPos.x);
            attackPoint.localPosition = apPos;
        }

        // Attack if in range
        if (distanceToPlayer <= detectionRange)
        {
            if (Time.time >= lastAttackTime + attackCooldown)
            {
                Attack();
            }
        }
    }

    // Called via Animation Event for smooth, accurate damage placement
    public void DealDamage()
    {
        if (attackPoint == null) return;

        Collider2D hit = Physics2D.OverlapCircle(attackPoint.position, attackCircleRadius, playerLayer);
        if (hit != null)
        {
            PlayerHealth playerHealth = hit.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(1);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        if (attackPoint != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(attackPoint.position, attackCircleRadius);
        }
    }

    void Attack()
    {
        lastAttackTime = Time.time;
        if (animator != null)
        {
            animator.speed = 1;
            animator.SetTrigger("Attack");
        }
    }

    public void ShrinkDown()
    {
        if (isDead) return;
        Die();
    }

    public void TakeDamage(int damage = 1)
    {
        if (isDead) return;

        health -= damage;
        StartCoroutine(HurtFlash());

        if (health <= 0) Die();
        else if (health <= 1) currentPhase = BossPhase.Frenzy;
        else if (health <= 3) currentPhase = BossPhase.Phase2;
    }

    IEnumerator HurtFlash()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = Color.white;
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;
        if (animator != null)
        {
            animator.speed = 1;
            animator.SetTrigger("Die");
        }
        
        if (bossCollider != null) bossCollider.enabled = false;
        if (tipCollider != null) tipCollider.enabled = false;

        if (BossCameraController.Instance != null)
            BossCameraController.Instance.ExitBossMode();

        Destroy(gameObject, 2f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isDead)
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null) playerHealth.TakeDamage(1);
        }
    }
}

