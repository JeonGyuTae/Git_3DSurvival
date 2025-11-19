using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class NodeSpawnEntry
{
    public GameObject nodePrefab;   // Pf_Node_Tree, Pf_Node_Rock 등
    public float weight = 1f;
}

/// <summary>
/// 나무/바위 같은 채집 노드 전용 스폰 포인트.
/// - 시작 시 즉시 노드를 하나 스폰.
/// - 노드가 파괴되면(respawnDelay 후) 다시 같은 위치에 스폰.
/// </summary>
public class NodeSpawner : MonoBehaviour
{
    public List<NodeSpawnEntry> spawnEntries = new List<NodeSpawnEntry>();
    public float respawnDelay = 120f;  // 나무/바위는 좀 더 길게

    private GameObject _currentNode;
    private float _timer;

    private void Start()
    {
        SpawnNow();
    }

    private void Update()
    {
        if (_currentNode != null) return;

        _timer -= Time.deltaTime;
        if (_timer > 0f) return;

        SpawnNow();
    }

    private void SpawnNow()
    {
        var entry = ChooseRandomEntry();
        if (entry == null || entry.nodePrefab == null)
            return;

        _currentNode = Instantiate(entry.nodePrefab, transform.position, Quaternion.identity);
        _currentNode.transform.SetParent(transform);
        _timer = respawnDelay;
    }

    private NodeSpawnEntry ChooseRandomEntry()
    {
        if (spawnEntries == null || spawnEntries.Count == 0)
            return null;

        float total = 0f;
        foreach (var e in spawnEntries)
            total += e.weight;

        float rnd = UnityEngine.Random.value * total;
        foreach (var e in spawnEntries)
        {
            rnd -= e.weight;
            if (rnd <= 0f)
                return e;
        }

        return spawnEntries[spawnEntries.Count - 1];
    }
}