using UnityEngine;

public class ForestAtmosphere : MonoBehaviour
{
    [Header("Falling Leaves")]
    public GameObject leafPrefab;
    public int leafCount = 20;
    public Vector2 spawnArea = new Vector2(20, 10);
    public Vector2 fallSpeed = new Vector2(1, 3);
    public Vector2 swayAmount = new Vector2(0.5f, 1.5f);

    private void Start()
    {
        for (int i = 0; i < leafCount; i++)
        {
            SpawnLeaf();
        }
    }

    void SpawnLeaf()
    {
        Vector3 spawnPos = transform.position + new Vector3(
            Random.Range(-spawnArea.x, spawnArea.x),
            Random.Range(-spawnArea.y, spawnArea.y),
            0
        );

        GameObject leaf = leafPrefab != null ? Instantiate(leafPrefab, spawnPos, Quaternion.identity) : CreateDefaultLeaf(spawnPos);
        leaf.transform.parent = transform;
        
        // Add a simple script to handle movement
        LeafMovement lm = leaf.AddComponent<LeafMovement>();
        lm.speed = Random.Range(fallSpeed.x, fallSpeed.y);
        lm.sway = Random.Range(swayAmount.x, swayAmount.y);
        lm.spawnArea = spawnArea;
        lm.parentSystem = this.transform;
    }

    GameObject CreateDefaultLeaf(Vector3 pos)
    {
        GameObject leaf = new GameObject("Leaf");
        leaf.transform.position = pos;
        SpriteRenderer sr = leaf.AddComponent<SpriteRenderer>();
        // Create a simple green square if no prefab is provided
        // In a real project, we'd use a leaf sprite
        sr.color = new Color(0.2f, 0.5f, 0.2f, 0.8f);
        sr.sortingOrder = 5;
        leaf.transform.localScale = Vector3.one * 0.2f;
        return leaf;
    }
}

public class LeafMovement : MonoBehaviour
{
    public float speed;
    public float sway;
    public Vector2 spawnArea;
    public Transform parentSystem;
    
    private float swayTime;

    void Update()
    {
        swayTime += Time.deltaTime;
        
        transform.position += Vector3.down * speed * Time.deltaTime;
        transform.position += Vector3.right * Mathf.Sin(swayTime * sway) * 0.5f * Time.deltaTime;
        transform.rotation = Quaternion.Euler(0, 0, Mathf.Sin(swayTime * sway) * 45f);

        // Respawn when out of bounds
        if (transform.position.y < parentSystem.position.y - spawnArea.y)
        {
            transform.position = new Vector3(
                parentSystem.position.x + Random.Range(-spawnArea.x, spawnArea.x),
                parentSystem.position.y + spawnArea.y,
                0
            );
        }
    }
}
