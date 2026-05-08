using UnityEngine;

public class ParallaxLayer : MonoBehaviour
{
    [Header("Camera")]
    public Transform cameraTransform;

    [Header("Parallax Settings")]
    [Range(0f, 1f)]
    public float parallaxFactor = 0.5f;

    public bool parallaxY = false;

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
        Vector3 cameraMovement = cameraTransform.position - cameraStartPosition;

        float newX = startPosition.x + cameraMovement.x * parallaxFactor;
        float newY = startPosition.y;

        if (parallaxY)
        {
            newY = startPosition.y + cameraMovement.y * parallaxFactor;
        }

        transform.position = new Vector3(newX, newY, startPosition.z);
    }
}