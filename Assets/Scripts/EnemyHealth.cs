using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Enemy Health")]
    [SerializeField] private int maxHealth = 3;

    private int currentHealth;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        Debug.Log($"{gameObject.name} took {damage} damage. HP left: {currentHealth}");

        Animator animator = GetComponent<Animator>();
        if (animator != null && currentHealth > 0)
        {
            animator.SetTrigger("Damage");
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log($"{gameObject.name} defeated.");
        
        // Prevent falling
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.gravityScale = 0;
            rb.bodyType = RigidbodyType2D.Kinematic;
        }

        Animator animator = GetComponent<Animator>();
        bool hasDieAnimation = false;
        
        if (animator != null)
        {
            foreach (var parameter in animator.parameters)
            {
                if (parameter.name == "Die")
                {
                    hasDieAnimation = true;
                    break;
                }
            }
        }

        if (hasDieAnimation)
        {
            animator.SetTrigger("Die");
            DisableEnemy();
            Destroy(gameObject, 2f); // Reduced from 15s to be more reasonable
        }
        else
        {
            DisableEnemy();
            Destroy(gameObject);
        }
    }

    private void DisableEnemy()
    {
        Collider2D[] colliders = GetComponents<Collider2D>();
        foreach (var col in colliders) col.enabled = false;
        
        MonoBehaviour[] scripts = GetComponents<MonoBehaviour>();
        foreach (var script in scripts)
        {
            if (script != this) script.enabled = false;
        }
        
        // Disable this script too so it's not counted by HUD/LevelManager
        this.enabled = false;
    }
}