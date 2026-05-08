using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    [Header("Patrol Settings")]
    public float speed = 2f;
    public float patrolDistance = 3f;

    private Vector3 startPosition;
    private int direction = 1;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        Patrol();
    }

    void Patrol()
    {
        transform.position += Vector3.right * direction * speed * Time.deltaTime;

        float distanceFromStart = transform.position.x - startPosition.x;

        if (distanceFromStart >= patrolDistance)
        {
            direction = -1;
            Flip();
        }
        else if (distanceFromStart <= -patrolDistance)
        {
            direction = 1;
            Flip();
        }
    }

    void Flip()
    {
        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * direction;
        transform.localScale = scale;
    }
}