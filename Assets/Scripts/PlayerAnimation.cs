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

        // PLACEHOLDER PARAMETERS
        animator.SetFloat("speed", speed);
        animator.SetBool("isJumping", rb.linearVelocity.y > 0.1f);
    }
}