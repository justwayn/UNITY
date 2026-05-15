using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int maxHealth = 3;

    private int currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        Debug.Log("Enemy took " + damage + " damage. HP left: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Enemy defeated.");
        Destroy(gameObject);
    }
}