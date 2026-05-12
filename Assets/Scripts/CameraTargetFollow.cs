using UnityEngine;

public class CameraTargetFollow : MonoBehaviour
{
    public Transform player;

    [Header("Offset")]
    public float offsetX = 0f;
    public float offsetY = 1.2f;

    void LateUpdate()
    {
        if (player == null) return;

        transform.position = new Vector3(
            player.position.x + offsetX,
            player.position.y + offsetY,
            transform.position.z
        );
    }
}