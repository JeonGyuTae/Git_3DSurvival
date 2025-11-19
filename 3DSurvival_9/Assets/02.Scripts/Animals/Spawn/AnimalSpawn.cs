using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Animal Spawn 스크립트
/// 
/// Area를 통해 생성되며 스폰할 Prefab을 가지고 있음
/// 
/// </summary>
public class AnimalSpawn : MonoBehaviour
{
    [Header("Animal Info")]
    [SerializeField] private AnimalData _animal;
    [SerializeField] private int maxAnimalCount;

    [Header("Spawn Info")]
    [SerializeField] private Vector3 spawnArea = new Vector3(50f, 5f, 50f);
    [SerializeField] private float offsetY = 10.0f;

    private PoolManager poolManager;
    private string key;

    private void Start()
    {
        poolManager = PoolManager.Instance;
        key = _animal.animalName;
        InitSpawn();
    }

    private void InitSpawn()
    {
        poolManager.CreatePool(key, _animal.prefab, maxAnimalCount);

        // 처음에는 최대로 소환
        for(int i=0; i<maxAnimalCount; i++)
        {
            Spawn();
        }
    }

    public void Spawn()
    {
        GameObject animal = poolManager.GetObject(key);
        NavMeshAgent agent = animal.GetComponent<NavMeshAgent>();

        // 위치 초기화
        // NavMeshAgent가 움직일 수 있는 영역 내에 Warp
        Vector3 spawnPosition;
        bool successWarp = false;
        do
        {
            spawnPosition = GetRandomPositionInArea();
            successWarp = agent.Warp(spawnPosition);

            if (!successWarp) Debug.Log($"{key} Warp 실패 위치 재설정");
        }
        while (successWarp == false);
    }

    public void Release(GameObject obj)
    {
        poolManager.ReleasePool(key, obj);
    }

    private Vector3 GetRandomPositionInArea()
    {
        Vector3 pivotPos = transform.position;

        // X, Z 축은 Area 영역 내 지정
        float randomPosX = Random.Range(pivotPos.x - spawnArea.x / 2, pivotPos.x + spawnArea.x / 2);
        float randomPosZ = Random.Range(pivotPos.z - spawnArea.z / 2, pivotPos.z + spawnArea.z / 2);

        // Y 축은 일정 offset 범위 위에서 지정 (땅 위에서 생성될 수 있도록)
        float posY = pivotPos.y + Random.Range(0, offsetY);

        // 위치 반환
        Vector3 spawnPos = new Vector3(randomPosX, posY, randomPosZ);
        return spawnPos;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = UnityEngine.Color.red;
        Gizmos.DrawWireCube(transform.position, spawnArea);
    }

    #region 프로퍼티

    public string Key {  get { return key; } }

    #endregion
}
