using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 3;
    public int currentHealth;

    [Header("UI")]
    public Image healthBarFill;

    [Header("Animation")]
    public Animator animator;

    [Header("Damage Settings")]
    public float invincibleTime = 1f;

    [Header("Death Settings")]
    public float defeatAnimationTime = 1.5f;

    private bool isInvincible;
    private bool isDead;

    private Rigidbody2D rb;
    private PlayerController playerController;
    private PlayerCombat playerCombat;
    private BoxCollider2D boxCollider;

    private Vector3 spawnPoint;

    private float originalGravityScale;

    void Start()
    {
        currentHealth = maxHealth;
        spawnPoint = transform.position;

        rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            originalGravityScale = rb.gravityScale;
        }

        playerController = GetComponent<PlayerController>();
        playerCombat = GetComponent<PlayerCombat>();
        boxCollider = GetComponent<BoxCollider2D>();

        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        UpdateHealthBar();
    }

    public void TakeDamage(int damage)
    {
        if (isInvincible || isDead) return;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        Debug.Log("Player HP: " + currentHealth);

        UpdateHealthBar();

        if (currentHealth <= 0)
        {
            StartCoroutine(DieRoutine());
            return;
        }

        PlayDamageAnimation();
        
        if (CameraShake.Instance != null)
        {
            CameraShake.Instance.Shake(0.15f, 0.2f);
        }

        StartCoroutine(InvincibilityRoutine());
    }

    void PlayDamageAnimation()
    {
        if (animator != null)
        {
            animator.SetTrigger("Damage");
        }
    }

    IEnumerator DieRoutine()
    {
        isDead = true;

        Debug.Log("Defeat triggered by PlayerHealth");

        if (GameManager.Instance != null)
             GameManager.Instance.isGameOver = true;

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
        }

        if (animator != null)
        {
            animator.ResetTrigger("Damage");
            animator.SetTrigger("Defeat");
        }

        yield return new WaitForSeconds(defeatAnimationTime);

        Respawn();
    }

    void Respawn()
    {
        isDead = false;
        if (GameManager.Instance != null)
             GameManager.Instance.isGameOver = false;

        currentHealth = maxHealth;
        UpdateHealthBar();

        transform.position = spawnPoint;

        if (rb != null)
        {
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            rb.linearVelocity = Vector2.zero;
            rb.gravityScale = originalGravityScale; // RESTORE ORIGINAL GRAVITY
        }

        if (animator != null)
        {
            animator.Rebind();
            animator.Update(0f);
        }

        boxCollider.enabled = true;
        
        Debug.Log("Player Respawned");
    }

    void UpdateHealthBar()
    {
        if (healthBarFill != null)
        {
            healthBarFill.fillAmount = (float)currentHealth / maxHealth;
        }
    }

    IEnumerator InvincibilityRoutine()
    {
        isInvincible = true;
        yield return new WaitForSeconds(invincibleTime);
        isInvincible = false;
    }
}