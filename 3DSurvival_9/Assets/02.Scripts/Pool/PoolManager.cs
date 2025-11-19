using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ObjectPool 모아놓은 전역 Manager 클래스
/// Dictionary 자료구조를 이용하여 key - value값을 가져온다.
/// 
/// 01. Pool<Chicken>   : 닭 종류의 Animal Pool
/// 
/// </summary>
public class PoolManager : MonoBehaviour 
{
    public static PoolManager Instance { get; private set; }

    private Dictionary<string, ObjectPool> objectPools;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            if(this != null)
                Destroy(gameObject);
        }
    }

    /// <summary>
    /// 게임에서 사용할 모든 종류의 ObjectPool 생성
    /// </summary>
    public void Init()
    {
        
    }

    /// <summary>
    /// ObjectPool 생성 및 Dictonary 추가
    /// </summary>
    /// <param name="key">Pool이름</param>
    /// <param name="prefab">원본 데이터</param>
    /// <param name="initialSize">생성 개수</param>
    public void CreatePool(string key, GameObject prefab, int initialSize) 
    {
        // 이미 존재하는지 확인
        if(objectPools.ContainsKey(key))
        {
            Debug.Log($"해당 {key}값 ObjectPool은 이미 존재합니다.");
            return;
        }

        // 부모 오브젝트 생성
        GameObject poolParent = new GameObject($"Pool_{key}");
        poolParent.transform.SetParent(this.transform);

        // ObjectPool 생성
        ObjectPool pool = new ObjectPool(prefab, initialSize, poolParent.transform);
        objectPools.Add(key, pool);
    }

    /// <summary>
    /// ObjectPool에서 오브젝트 가져오기
    /// </summary>
    /// <param name="key">Pool이름</param>
    /// <returns>해당 오브젝트 or null</returns>
    public GameObject GetObject(string key) 
    {
        // Dictionary에서 key에 해당하는 ObjectPool을 가져옴
        if (objectPools.TryGetValue(key, out ObjectPool pool))
        {
            return pool.Get(); 
        }

        Debug.Log($"해당 {key}값을 가지고 있는 ObjectPool이 존재하지 않습니다.");
        return null;
    }

    /// <summary>
    /// ObjectPool Release 및 비활성화
    /// </summary>
    /// <param name="key">Pool이름</param>
    /// <param name="obj">오브젝트</param>
    public void ReleasePool(string key, GameObject obj) 
    {
        // Dictionary에서 key에 해당하는 ObjectPool을 가져옴
        if (objectPools.TryGetValue(key, out ObjectPool pool))
        {
            pool.Release(obj); // ObjectPool의 Release() 메서드 호출
        }
        else
        {
            // Pool이 없으면 오브젝트를 강제로 비활성화하고 로그 출력
            if (obj != null)
            {
                obj.gameObject.SetActive(false);
            }
            Debug.LogError($"해당 {key}값을 가지고 있는 ObjectPool이 존재하지 않습니다.");
        }
    }
}
