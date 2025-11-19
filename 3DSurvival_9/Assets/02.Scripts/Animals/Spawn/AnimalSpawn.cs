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

    private void Start()
    {
        InitSpawn();
    }

    private void InitSpawn()
    {
        PoolManager.Instance.CreatePool(_animal.animalName, _animal.prefab, maxAnimalCount);

        Spawn();
    }

    public void Spawn()
    {
        GameObject animal = PoolManager.Instance.GetObject(_animal.animalName);
        NavMeshAgent agent = animal.GetComponent<NavMeshAgent>();

        // 위치 초기화
        Vector3 spawnPosition = GetRandomPositionInArea();

        if(agent != null)
        {
            if(!agent.Warp(spawnPosition))
            {
                Debug.Log($"{_animal.animalName} Warp 실패.");
            }
        }
    }

    private Vector3 GetRandomPositionInArea()
    {
        Vector3 pivotPos = transform.position;

        // X, Z 축은 Area 영역 내 지정
        float randomPosX = Random.Range(pivotPos.x - spawnArea.x / 2, pivotPos.x + spawnArea.x / 2);
        float randomPosZ = Random.Range(pivotPos.z - spawnArea.z / 2, pivotPos.z + spawnArea.z / 2);

        // Y 축은 일정 offset 범위 위에서 지정 (땅 위에서 생성될 수 있도록)
        float posY = pivotPos.y + offsetY;

        // 위치 반환
        Vector3 spawnPos = new Vector3(randomPosX, posY, randomPosZ);
        return spawnPos;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = UnityEngine.Color.red;
        Gizmos.DrawWireCube(transform.position, spawnArea);
    }
}
