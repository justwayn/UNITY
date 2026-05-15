using UnityEngine;

public class ParallaxSystem : MonoBehaviour
{
    [System.Serializable]
    public struct ParallaxLayerData
    {
        public Transform layerTransform;
        public float parallaxFactor; // 0 = moves with camera, 1 = static
        public bool infiniteHorizontal;
    }

    public ParallaxLayerData[] layers;
    
    private Transform cam;
    private Vector3 lastCamPos;

    void Start()
    {
        cam = Camera.main.transform;
        lastCamPos = cam.position;

        // Initialize layer widths for infinite scrolling
        for (int i = 0; i < layers.Length; i++)
        {
            if (layers[i].infiniteHorizontal && layers[i].layerTransform != null)
            {
                // Assuming the layer has a SpriteRenderer
                SpriteRenderer sr = layers[i].layerTransform.GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    // You might want to duplicate the sprite to ensure seamless looping
                    // But for now, we'll just track the width
                }
            }
        }
    }

    void LateUpdate()
    {
        Vector3 deltaMovement = cam.position - lastCamPos;
        
        for (int i = 0; i < layers.Length; i++)
        {
            if (layers[i].layerTransform == null) continue;

            float parallaxSpeed = layers[i].parallaxFactor;
            layers[i].layerTransform.position += new Vector3(deltaMovement.x * parallaxSpeed, deltaMovement.y * (parallaxSpeed * 0.5f), 0);
            
            // Infinite horizontal scrolling logic
            if (layers[i].infiniteHorizontal)
            {
                SpriteRenderer sr = layers[i].layerTransform.GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    float textureUnitSizeX = sr.sprite.texture.width / sr.sprite.pixelsPerUnit;
                    float scaleX = layers[i].layerTransform.localScale.x;
                    float totalWidth = textureUnitSizeX * scaleX;

                    if (Mathf.Abs(cam.position.x - layers[i].layerTransform.position.x) >= totalWidth)
                    {
                        float offsetPositionX = (cam.position.x - layers[i].layerTransform.position.x) % totalWidth;
                        layers[i].layerTransform.position = new Vector3(cam.position.x + offsetPositionX, layers[i].layerTransform.position.y);
                    }
                }
            }
        }

        lastCamPos = cam.position;
    }
}
