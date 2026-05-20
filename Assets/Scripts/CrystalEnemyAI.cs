using UnityEngine;

public class CrystalEnemyAI : MonoBehaviour
{
    public int health = 3;
    private Animator animator;
    private bool isDead = false;

    [Header("Ground Check")]
    public LayerMask groundLayer;
    public float checkDistance = 1f;
    private bool isOnGround = false;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (isDead) return;

        // Use Raycast to check if it's on the ground, filtering only the Ground layer
        // This prevents floating platforms (on other layers) from interfering
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, checkDistance, groundLayer);
        isOnGround = hit.collider != null;

        if (!isOnGround)
        {
            // Optional: Handle crystal logic if not on ground (e.g., fall or deactivate)
        }
    }

    public void TakeDamage(int damage = 1)
    {
        if (isDead) return;
        health -= damage;
        if (health <= 0) Die();
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

        Collider2D[] colliders = GetComponents<Collider2D>();
        foreach (var col in colliders) col.enabled = false;

        // The TentacleBossAI will now check for remaining crystals in its own Update loop
        // as requested, ensuring a more reliable death trigger.
        Destroy(gameObject, 1.5f); 
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * checkDistance);
    }
}
