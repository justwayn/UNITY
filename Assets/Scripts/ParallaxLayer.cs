using UnityEngine;

public class ParallaxLayer : MonoBehaviour
{
    [Header("Camera")]
    public Transform cameraTransform;

    [Header("Parallax Amount")]
    [Range(0f, 1f)]
    public float parallaxX = 0.3f;

    [Range(0f, 1f)]
    public float parallaxY = 0f;

    private Vector3 startPosition;
    private Vector3 cameraStartPosition;

    void Start()
    {
        if (cameraTransform == null)
        {
            cameraTransform = Camera.main.transform;
        }

        startPosition = transform.position;
        cameraStartPosition = cameraTransform.position;
    }

    void LateUpdate()
    {
        Vector3 cameraDelta = cameraTransform.position - cameraStartPosition;

        float newX = startPosition.x + cameraDelta.x * parallaxX;
        float newY = startPosition.y + cameraDelta.y * parallaxY;

        transform.position = new Vector3(newX, newY, startPosition.z);
    }
}