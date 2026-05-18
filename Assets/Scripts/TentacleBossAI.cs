using UnityEngine;
using System.Collections;

public class TentacleBossAI : MonoBehaviour
{
    [Header("Health")]
    public int health = 5;
    private int maxHealth;

    [Header("Movement")]
    public float moveSpeed = 2f;
    public float frenzySpeed = 4f;

    [Header("Combat Settings")]
    public float detectionRange = 12f;
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
        if (rb != null) rb.bodyType = RigidbodyType2D.Kinematic;

        // Player can pass through tentacles
        if (bossCollider != null) bossCollider.isTrigger = true;

        Transform tip = transform.Find("TentacleTip");
        if (tip != null)
        {
            tipCollider = tip.GetComponent<Collider2D>();
            tipCollider.enabled = false; // Start hidden
        }

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;
    }

    void Update()
    {
        if (isDead) return;

        // Find player if not assigned
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) player = playerObj.transform;
        }

        if (player != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);

            // Flip to face player
            Vector3 localScale = transform.localScale;
            if (player.position.x > transform.position.x)
            {
                // Player is on the right, ensure scale.x is positive
                localScale.x = Mathf.Abs(localScale.x);
            }
            else
            {
                // Player is on the left, ensure scale.x is negative
                localScale.x = -Mathf.Abs(localScale.x);
            }
            transform.localScale = localScale;

            // Attack if in range
            if (distanceToPlayer <= detectionRange)
            {
                if (Time.time >= lastAttackTime + attackCooldown)
                {
                    Attack();
                }
            }
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
        Die(); // Play death animation instead of shrinking
    }

    IEnumerator ShrinkRoutine()
    {
        // This routine is now bypassed by ShrinkDown calling Die() directly
        yield break;
    }

    IEnumerator BossBehaviorLoop()
    {
        while (!isDead && currentPhase != BossPhase.Frenzy)
        {
            int attackCount = 3;
            float interval = (currentPhase == BossPhase.Phase1) ? attackInterval : attackIntervalPhase2;

            for (int i = 0; i < attackCount; i++)
            {
                animator.SetTrigger("Attack");
                yield return new WaitForSeconds(interval);
            }

            // Expose Core
            isExposed = true;
            if (tipCollider != null) tipCollider.enabled = true;
            spriteRenderer.color = Color.white; // Ensure normal color

            float exposeDuration = (currentPhase == BossPhase.Phase1) ? phase1ExposeTime : phase2ExposeTime;
            yield return new WaitForSeconds(exposeDuration);

            isExposed = false;
            if (tipCollider != null) tipCollider.enabled = false;
        }
    }

    public void TakeDamage(int damage = 1)
    {
        if (isDead) return;

        health -= damage;
        StartCoroutine(HurtFlash());

        if (health <= 0)
        {
            Die();
        }
        else if (health <= 1)
        {
            currentPhase = BossPhase.Frenzy;
        }
        else if (health <= 3)
        {
            currentPhase = BossPhase.Phase2;
        }
    }

    IEnumerator HurtFlash()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = Color.white;
    }

    void Die()
    {
        isDead = true;
        if (animator != null)
        {
            animator.speed = 1;
            animator.SetTrigger("Die");
        }
        
        if (bossCollider != null) bossCollider.enabled = false;
        if (tipCollider != null) tipCollider.enabled = false;

        // 4d. Exit Boss Mode
        if (BossCameraController.Instance != null)
        {
            BossCameraController.Instance.ExitBossMode();
        }

        // Keep death sprite visible
        Destroy(gameObject, 15f);
        }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(1);
            }
        }
    }
}
