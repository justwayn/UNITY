using UnityEngine;
using System.Collections;

public class CrystalEnemyAI : MonoBehaviour
{
    public int health = 3;
    private Animator animator;
    private bool isDead = false;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update() { }

    public void TakeDamage(int damage = 1)
    {
        if (isDead) return;
        health -= damage;
        if (health <= 0) Die();
    }

    void Die()
    {
        isDead = true;
        if (animator != null)
        {
            animator.speed = 1;
            animator.SetTrigger("Die");
        }

        Collider2D[] colliders = GetComponents<Collider2D>();
        foreach (var col in colliders) col.enabled = false;

        TentacleBossAI[] allBosses = Object.FindObjectsByType<TentacleBossAI>(FindObjectsSortMode.None);
        foreach (var boss in allBosses) boss.ShrinkDown();

        // Ensure destruction is long enough to see animation
        Destroy(gameObject, 15f); 
    }
}