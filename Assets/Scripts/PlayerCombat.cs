using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("Attack Settings")]
    public Transform attackPoint;
    public float attackRange = 1f;
    public LayerMask enemyLayer;
    public int damage = 1;
    public float attackCooldown = 0.5f;

    [Header("Animation")]
    public Animator animator;

    private float nextAttackTime;


    void Awake()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
    }

    void Update()
    {
        // Don't process input if dead or game over
        if (GameManager.Instance != null && GameManager.Instance.isGameOver) return;

        // Attack 1 (Left Click)
        if (Input.GetMouseButtonDown(0) && Time.time >= nextAttackTime)
        {
            Attack(0);
            nextAttackTime = Time.time + attackCooldown;
        }

        // Attack 2 (Right Click)
        if (Input.GetMouseButtonDown(1) && Time.time >= nextAttackTime)
        {
            Attack(1);
            nextAttackTime = Time.time + attackCooldown;
        }
    }

    void Attack(int step)
    {
        if (animator != null)
        {
            animator.SetInteger("AttackNum", step);
            animator.SetTrigger("isAttacking");
        }
    }

    // Call this using Animation Event
    public void DealAttackDamage()
    {
        Debug.Log("Player attack damage triggered");
        if (attackPoint == null)
        {
            Debug.LogWarning("AttackPoint is missing.");
            return;
        }

        Collider2D[] enemies = Physics2D.OverlapCircleAll(
            attackPoint.position,
            attackRange,
            enemyLayer
        );

        foreach (Collider2D enemy in enemies)
        {
            // Try different health/AI components
            Enemy e = enemy.GetComponentInParent<Enemy>();
            if (e != null)
            {
                e.TakeDamage(damage);
                continue;
            }

            EnemyHealth eh = enemy.GetComponentInParent<EnemyHealth>();
            if (eh != null)
            {
                eh.TakeDamage(damage);
                continue;
            }

            CrystalEnemyAI ce = enemy.GetComponentInParent<CrystalEnemyAI>();
            if (ce != null)
            {
                ce.TakeDamage(damage);
                continue;
            }

            TentacleBossAI tb = enemy.GetComponentInParent<TentacleBossAI>();
            if (tb != null)
            {
                tb.TakeDamage(damage);
                continue;
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}