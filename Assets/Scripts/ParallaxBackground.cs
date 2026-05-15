using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    [Header("Parallax Settings")]
    public float parallaxFactor; // 0 = moves with camera, 1 = static
    public bool infiniteHorizontal = true;
    
    [Header("Drift Settings")]
    public float driftSpeed;

    private Transform cameraTransform;
    private Vector3 lastCameraPosition;
    private float textureUnitSizeX;

    void Start()
    {
        cameraTransform = Camera.main.transform;
        lastCameraPosition = cameraTransform.position;

        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null && spriteRenderer.sprite != null)
        {
            Sprite sprite = spriteRenderer.sprite;
            textureUnitSizeX = sprite.texture.width / sprite.pixelsPerUnit;
        }
    }

    void LateUpdate()
    {
        // 1. Apply movement based on camera delta
        Vector3 deltaMovement = cameraTransform.position - lastCameraPosition;
        // transform.position += deltaMovement * parallaxFactor;
        // Conventional parallax: factor 0 = static (far), factor 1 = moves with player (near)
        // Let's use: movement = delta * factor
        transform.position += new Vector3(deltaMovement.x * parallaxFactor, deltaMovement.y * (parallaxFactor * 0.2f), 0);

        // 2. Apply auto-drift
        transform.position += Vector3.right * driftSpeed * Time.deltaTime;

        lastCameraPosition = cameraTransform.position;

        // 3. Infinite scrolling (if using a renderer that isn't already huge)
        // With DrawMode.Tiled and a size of 500, we don't need to wrap often.
        if (infiniteHorizontal && textureUnitSizeX > 0)
        {
            if (Mathf.Abs(cameraTransform.position.x - transform.position.x) >= textureUnitSizeX)
            {
                float offsetPositionX = (cameraTransform.position.x - transform.position.x) % textureUnitSizeX;
                transform.position = new Vector3(cameraTransform.position.x + offsetPositionX, transform.position.y, transform.position.z);
            }
        }
    }
}
