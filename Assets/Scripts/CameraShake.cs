using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void Shake(float duration, float magnitude)
    {
        // Manual position modification fights with Cinemachine.
        // For now, we disable it to stop the jittering.
        // To implement properly, use Cinemachine Impulse or Noise.
        Debug.Log("Camera Shake suppressed to prevent jittering with Cinemachine.");
    }
}