using UnityEngine;

public class BossTriggerZone : MonoBehaviour
{
    private bool hasTriggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (BossCameraController.Instance != null)
            {
                BossCameraController.Instance.EnterBossMode();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (BossCameraController.Instance != null)
            {
                BossCameraController.Instance.ExitBossMode();
            }
        }
    }
}
