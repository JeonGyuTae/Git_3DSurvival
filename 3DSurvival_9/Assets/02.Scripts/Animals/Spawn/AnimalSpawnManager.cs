using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// AnimalSpawn을 관리하는 스크립트
/// 
/// Animal이 죽었을 때 일정 시간 지나면 리스폰하게 도와줌
/// 
/// </summary>
public class AnimalSpawnManager : MonoBehaviour
{
    public static AnimalSpawnManager Instance { get; private set; }


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private AnimalSpawn[] animalSpawns;

    private void Start()
    {
        // 자식 오브젝트에 있는 AnimalSpawn을 가져옴
        animalSpawns = GetComponentsInChildren<AnimalSpawn>();
    }

    public void Spawn(string key)
    {
        foreach (var spawn in animalSpawns)
        {
            if (spawn.Key == key)
            {
                spawn.Spawn();
                Debug.Log($"{key} 제거");

                // 리스폰 해야함 -> 코루틴 사용 (개별?)

                return;
            }
        }

        Debug.Log($"{key} key 값에 해당하는 objectPool이 존재하지 않습니다.");
    }

    public void Release(string key, GameObject obj)
    {
        foreach(var spawn in animalSpawns)
        {
            if (spawn.Key == key)
            {
                spawn.Release(obj);
                Debug.Log($"{key} 제거");
                return;
            }
        }

        Debug.Log($"{key} key 값에 해당하는 objectPool이 존재하지 않습니다.");
    }

    public void Respawn(Animal animal)
    {
        StartCoroutine(RespawnCoroutine(animal));
    }

    private IEnumerator RespawnCoroutine(Animal animal)
    {
        yield return new WaitForSeconds(animal.Data.respawnTime);

        Debug.Log($"{animal.Data.animalName} 리스폰");
        
        // 초기화
        animal.Init();

        // 리스폰
        Spawn(animal.Data.animalName);
    }
}
