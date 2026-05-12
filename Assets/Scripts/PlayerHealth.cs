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

    void Start()
    {
        currentHealth = maxHealth;

        rb = GetComponent<Rigidbody2D>();
        playerController = GetComponent<PlayerController>();
        playerCombat = GetComponent<PlayerCombat>();

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

        if (playerController != null)
            playerController.enabled = false;

        if (playerCombat != null)
            playerCombat.enabled = false;

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.gravityScale = 0f;
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
        }

        if (animator != null)
        {
            animator.ResetTrigger("Damage");
            animator.SetTrigger("Defeat");
        }

        yield return new WaitForSeconds(defeatAnimationTime);

        Debug.Log("Player defeated");

        gameObject.SetActive(false);
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