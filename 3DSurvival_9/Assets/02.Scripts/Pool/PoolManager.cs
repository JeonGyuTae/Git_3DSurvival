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

    private Dictionary<string, object> objectPools;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
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
    /// <typeparam name="T">오브젝트의 타입</typeparam>
    /// <param name="key">Pool이름</param>
    /// <param name="prefab">원본 데이터</param>
    /// <param name="initialSize">생성 개수</param>
    public void CreatePool<T>(string key, T prefab, int initialSize) where T : MonoBehaviour
    {
        // 이미 존재하는지 확인
        if(objectPools.ContainsKey(key))
        {
            Debug.Log($"{key} Pool은 이미 존재하는 ObjectPool 입니다.");
            return;
        }

        // 부모 오브젝트 생성
        GameObject poolParent = new GameObject($"Pool_{key}");
        poolParent.transform.SetParent(this.transform);

        // ObjectPool 생성
        ObjectPool<T> pool = new ObjectPool<T>(prefab, initialSize, poolParent.transform);
        objectPools.Add(key, pool);
    }

    /// <summary>
    /// ObjectPool에서 오브젝트 가져오기
    /// </summary>
    /// <typeparam name="T">오브젝트의 타입</typeparam>
    /// <param name="key">Pool이름</param>
    /// <returns>해당 오브젝트 or null</returns>
    public T GetObject<T>(string key) where T : MonoBehaviour
    {
        if (objectPools.TryGetValue(key, out object poolObject))
        {
            // object를 ObjectPool<T> 타입으로 변환 (캐스팅)
            if (poolObject is ObjectPool<T> pool)
            {
                return pool.Get();
            }
            else
            {
                Debug.Log($"해당 {key}값을 가지고 있는 ObjectPool이 존재하지 않습니다.");
                return null;
            }
        }

        Debug.Log($"해당 {key}값을 가지고 있는 ObjectPool이 존재하지 않습니다.");
        return null;
    }

    /// <summary>
    /// ObjectPool Release 및 비활성화
    /// </summary>
    /// <typeparam name="T">오브젝트의 타입</typeparam>
    /// <param name="key">Pool이름</param>
    /// <param name="obj">오브젝트</param>
    public void ReleasePool<T>(string key, T obj) where T : MonoBehaviour
    {
        if(objectPools.TryGetValue(key, out object poolObj))
        {
            if (poolObj is ObjectPool<T> pool)
            {
                pool.Release(obj);
            }
            else
            {
                Debug.Log("해당 key값을 가지고 있는 ObjectPool이 존재하지 않습니다.");
            }
        }
        else
        {
            obj.gameObject.SetActive(false);
            Debug.Log("해당 key값을 가지고 있는 ObjectPool이 존재하지 않습니다.");
        }
    }
}
