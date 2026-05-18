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

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log($"{gameObject.name} defeated.");
        
        Animator animator = GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetTrigger("Die");
            // Disable colliders and AI scripts to "show" the death
            Collider2D[] colliders = GetComponents<Collider2D>();
            foreach (var col in colliders) col.enabled = false;
            
            MonoBehaviour[] scripts = GetComponents<MonoBehaviour>();
            foreach (var script in scripts)
            {
                if (script != this) script.enabled = false;
            }

            Destroy(gameObject, 15f); // Delay destruction to show the animation
            }
            else
            {
            Destroy(gameObject, 15f);
            }
            }
}