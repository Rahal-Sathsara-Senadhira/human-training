using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;

    [Tooltip("Enemy prefabs to spawn (drag your Enemy1_Prefab here).")]
    [SerializeField] private List<GameObject> enemyPrefabs = new List<GameObject>();

    [Tooltip("Spawn points (transforms in your scene).")]
    [SerializeField] private List<Transform> spawnPoints = new List<Transform>();

    [Header("World Health Bar (Optional)")]
    [SerializeField] private WorldHealthBar worldHealthBarPrefab;
    [SerializeField] private Vector3 healthBarOffset = new Vector3(0f, 2.1f, 0f);

    [Header("Spawn Settings")]
    [SerializeField] private bool spawnOnStart = true;
    [SerializeField] private float spawnDelay = 0.5f;

    private Camera mainCam;

    void Awake()
    {
        mainCam = Camera.main;
    }

    void Start()
    {
        if (spawnOnStart)
            Invoke(nameof(SpawnOne), spawnDelay);
    }

    public void SpawnOne()
    {
        if (enemyPrefabs == null || enemyPrefabs.Count == 0)
        {
            Debug.LogWarning("[EnemySpawner] No enemy prefabs assigned.");
            return;
        }

        if (spawnPoints == null || spawnPoints.Count == 0 || spawnPoints[0] == null)
        {
            Debug.LogWarning("[EnemySpawner] No spawn points assigned.");
            return;
        }

        GameObject prefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Count)];
        Transform spawn = spawnPoints[Random.Range(0, spawnPoints.Count)];

        if (!prefab || !spawn) return;

        GameObject enemyGO = Instantiate(prefab, spawn.position, spawn.rotation);

        // Hook player into AI
        EnemyAI ai = enemyGO.GetComponent<EnemyAI>();
        if (ai && player) ai.SetPlayer(player);

        // Optional: spawn health bar
        if (worldHealthBarPrefab)
        {
            Health h = enemyGO.GetComponentInChildren<Health>();
            if (!h) h = enemyGO.GetComponent<Health>();

            // Create a follow point above head
            GameObject follow = new GameObject("HealthBar_Follow");
            follow.transform.SetParent(enemyGO.transform);
            follow.transform.localPosition = healthBarOffset;

            WorldHealthBar bar = Instantiate(worldHealthBarPrefab, follow.transform.position, Quaternion.identity);
            bar.name = "WorldHealthBar";
            bar.Init(h, follow.transform, mainCam);
        }
    }
}
