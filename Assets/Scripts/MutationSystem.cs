using UnityEngine;

public class MutationSystem : MonoBehaviour
{
    public float power = 0f;
    public float maxPower = 100f;

    public float speedBoost = 2f;

    private PlayerController player;

    void Start()
    {
        player = GetComponent<PlayerController>();
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.K))
        {
            ActivateMutation();
        }
        else
        {
            DeactivateMutation();
        }
    }

    void ActivateMutation()
    {
        power += Time.deltaTime * 10;
        player.speed = 5f + speedBoost;

        if (power >= maxPower)
        {
            Die();
        }
    }

    void DeactivateMutation()
    {
        player.speed = 5f;
    }

    void Die()
    {
        Debug.Log("Mutation Overload!");
        Time.timeScale = 0;
    }
}