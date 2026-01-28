using UnityEngine;

public class HealthBarSpawner : MonoBehaviour
{
    public GameObject worldHealthBarPrefab;

    public Transform target;
    public MonoBehaviour health;   // auto finds script named "Health"
    public Camera faceCamera;

    private WorldHealthBar spawned;

    private void Start()
    {
        SpawnNow();
    }

    public void SpawnNow()
    {
        if (spawned != null) return;

        if (worldHealthBarPrefab == null)
        {
            Debug.LogWarning("[HealthBarSpawner] worldHealthBarPrefab not assigned.");
            return;
        }

        if (target == null) target = transform;

        if (health == null)
        {
            var all = GetComponents<MonoBehaviour>();
            foreach (var mb in all)
            {
                if (mb != null && mb.GetType().Name == "Health")
                {
                    health = mb;
                    break;
                }
            }
        }

        if (faceCamera == null) faceCamera = Camera.main;

        var go = Instantiate(worldHealthBarPrefab);
        spawned = go.GetComponent<WorldHealthBar>();

        if (spawned == null)
        {
            Debug.LogWarning("[HealthBarSpawner] Prefab missing WorldHealthBar script.");
            Destroy(go);
            return;
        }

        spawned.Init(health, target, faceCamera);
    }
}
