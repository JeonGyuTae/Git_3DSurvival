using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ResourceSpawnEntry
{
    public ResourceData resourceData;   // 어떤 자원인지 (정보용)
    public GameObject resourcePrefab;   // 어떤 프리팹을 스폰할지
    public float weight = 1f;           // 랜덤 비율
}

/// <summary>
/// 이 컴포넌트를 가진 오브젝트를 '스폰존 프리팹'으로 만들고
/// 맵에 배치하면 size 박스 영역 안에서 자원들이 랜덤 위치로 스폰된다.
/// </summary>
public class ResourceSpawnArea : MonoBehaviour
{
    [Header("Area Size")]
    public Vector3 size = new Vector3(10, 1, 10);   // 이 박스 안에서 스폰

    [Header("Spawn Rules")]
    public List<ResourceSpawnEntry> spawnEntries = new List<ResourceSpawnEntry>();

    [Header("Settings")]
    public int maxResourceCount = 10;         // 동시에 존재 가능한 최대 자원 개수
    public float checkIntervalSeconds = 5f;   // 몇 초마다 스폰 체크할지

    private float _timer;

    private void Update()
    {
        _timer -= Time.deltaTime;
        if (_timer > 0f) return;

        _timer = checkIntervalSeconds;

        // 현재 자식 오브젝트 중 active인 개수 세기
        int current = 0;
        foreach (Transform child in transform)
        {
            if (child.gameObject.activeInHierarchy)
                current++;
        }

        if (current >= maxResourceCount)
            return;

        SpawnOne();
    }

    private void SpawnOne()
    {
        var entry = ChooseRandomEntry();
        if (entry == null || entry.resourcePrefab == null)
            return;

        Vector3 pos = GetRandomPositionInArea();
        Quaternion rot = Quaternion.identity;

        Instantiate(entry.resourcePrefab, pos, rot, transform);
    }

    private ResourceSpawnEntry ChooseRandomEntry()
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

    private Vector3 GetRandomPositionInArea()
    {
        Vector3 c = transform.position;
        return new Vector3(
            c.x + UnityEngine.Random.Range(-size.x / 2f, size.x / 2f),
            c.y,
            c.z + UnityEngine.Random.Range(-size.z / 2f, size.z / 2f)
        );
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, size);
    }
}
