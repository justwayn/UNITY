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
    private int comboStep = 0;

    void Awake()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
    }

    void Update()
    {
        // Left click attack
        if (Input.GetMouseButtonDown(0) && Time.time >= nextAttackTime)
        {
            Attack();
            nextAttackTime = Time.time + attackCooldown;
        }
    }

    void Attack()
    {
        if (comboStep == 0)
        {
            animator.ResetTrigger("Attack1");
            animator.SetTrigger("Attack");
            comboStep = 1;
        }
        else
        {
            animator.ResetTrigger("Attack");
            animator.SetTrigger("Attack1");
            comboStep = 0;
        }
    }

    // Call this using Animation Event
    public void DealAttackDamage()
    {
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
            enemy.GetComponent<Enemy>()?.TakeDamage(damage);
        }
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}