using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class ResourceSpawnEntry
{
    public ResourceData resourceData;   // 어떤 자원인지 (정보용)
    public GameObject resourcePrefab;   // 어떤 프리팹을 스폰할지
    public float weight = 1f;           // 랜덤 비율
}

///
/// 이 컴포넌트를 가진 오브젝트를 '스폰존 프리팹'으로 만들고
/// 맵에 배치하면 size 박스 영역 안에서 자원들이 랜덤 위치로 스폰된다
///
/// - 씬 처음 시작할 때:
///   → maxResourceCount까지 꽉 채워서 초기 스폰
/// - 이후:
///   → 자원이 줄어들면 checkIntervalSeconds마다 1개씩 랜덤 위치로 리스폰
///   
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

    private void Start()
    {
        // 처음 씬에 들어왔을 때, 영역 안을 maxResourceCount까지 꽉 채운다.
        int current = CountCurrentResources();
        int toSpawn = Mathf.Max(0, maxResourceCount - current);

        for (int i = 0; i < toSpawn; i++)
        {
            SpawnOne();
        }

        _timer = checkIntervalSeconds;
    }

    private void Update()
    {
        // 이후에는 일정 주기마다 개수를 체크해서 부족하면 1개씩 리스폰
        _timer -= Time.deltaTime;
        if (_timer > 0f) return;

        _timer = checkIntervalSeconds;

        int current = CountCurrentResources();
        if (current >= maxResourceCount)
            return;

        // 개수가 부족하면 1개씩 천천히 채운다.
        SpawnOne();
    }

    /// 
    /// 현재 스폰존 안에 살아있는 자원 개수 세기
    /// (자식 오브젝트 기준)
    /// 
    private int CountCurrentResources()
    {
        int count = 0;
        foreach (Transform child in transform)
        {
            if (child.gameObject.activeInHierarchy)
                count++;
        }
        return count;
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


    [Header("Ground")]
    [Tooltip("자원이 올라앉을 바닥 레이어")]
    public LayerMask groundMask = ~0;
    private Vector3 GetRandomPositionInArea()
    {
        // XZ는 기존 방식대로 랜덤
        Vector3 center = transform.position;
        float randX = Random.Range(-size.x / 2f, size.x / 2f);
        float randZ = Random.Range(-size.z / 2f, size.z / 2f);

        //대략적인 위치
        Vector3 pos = new Vector3(
            center.x + randX,
            center.y + size.y / 2f + 5f,   // 박스 위 + 약간 위에서 쏘기
            center.z + randZ
        );

        //위에서 아래로 레이캐스트 쏴서 바닥 찾기
        if (Physics.Raycast(pos, Vector3.down, out RaycastHit hit, 100f, groundMask))
        {
            return hit.point;  // 바닥에 딱 붙이기
        }

        //바닥을 못 찾으면, 그냥 원래 높이로 반환
        return new Vector3(
            center.x + randX,
            center.y,
            center.z + randZ
        );
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, size);
    }
}