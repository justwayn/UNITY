using System.Collections;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    [SerializeField] private int damage = 1;
    [SerializeField] private float attackRange = 1f;
    [SerializeField] private float attackCooldown = 0.5f;

    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private LayerMask enemyLayer;

    private bool canAttack = true;

    private void Awake()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && canAttack)
        {
            StartCoroutine(AttackRoutine("Attack1"));
        }

        if (Input.GetMouseButtonDown(1) && canAttack)
        {
            StartCoroutine(AttackRoutine("Attack2"));
        }
    }

    private IEnumerator AttackRoutine(string attackTrigger)
    {
        canAttack = false;

        animator.SetTrigger(attackTrigger);

        yield return new WaitForSeconds(attackCooldown);

        canAttack = true;
    }

    // Animation Event calls this during the hit frame
    public void DealAttackDamage()
    {
        if (attackPoint == null)
        {
            Debug.LogWarning("AttackPoint is not assigned.");
            return;
        }

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(
            attackPoint.position,
            attackRange,
            enemyLayer
        );

        foreach (Collider2D enemy in hitEnemies)
        {
            EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();

            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);
            }
        }
    }

    // Animation Event calls this at the last frame
    public void EndAttack()
    {
        // For now empty lang muna.
        // Useful later if you want to block movement while attacking.
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;

        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}