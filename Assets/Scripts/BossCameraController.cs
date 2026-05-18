using UnityEngine;
using Unity.Cinemachine;

public class BossCameraController : MonoBehaviour
{
    public static BossCameraController Instance { get; private set; }

    public CinemachineCamera playerFollowCamera;
    public CinemachineCamera bossCamera;
    public float bossOrthoSize = 16f;
    public GameObject arenaWalls;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            if (bossCamera != null)
            {
                bossCamera.Lens.OrthographicSize = bossOrthoSize;
                // Make boss camera static (not following player)
                bossCamera.Follow = null; 
                bossCamera.LookAt = null;
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void EnterBossMode()
    {
        if (bossCamera != null)
        {
            bossCamera.Priority = 20;
            if (arenaWalls != null) arenaWalls.SetActive(true);
        }
    }

    public void ExitBossMode()
    {
        if (bossCamera != null)
        {
            bossCamera.Priority = 5;
            if (arenaWalls != null) arenaWalls.SetActive(false);
        }
    }
}
