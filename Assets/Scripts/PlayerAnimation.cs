using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private Animator animator;
    private Rigidbody2D rb;

    void Start()
    {
        animator = GetComponent<Animator>(); 
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        HandleAnimations();
    }

    void HandleAnimations()
    {
        float speed = Mathf.Abs(rb.linearVelocity.x);

        // Updated to match New Player Animator
        animator.SetFloat("speed", speed);
        animator.SetBool("isRunning", speed > 0.1f);
        animator.SetBool("isGrounded", Mathf.Abs(rb.linearVelocity.y) < 0.01f);
    }
}