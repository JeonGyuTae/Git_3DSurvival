using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool
{
    private Queue<GameObject> pool = new Queue<GameObject>();       // 재활용 오브젝트를 담을 Queue
    private GameObject prefab;                                      // 복사하여 사용할 원본 오브젝트
    private Transform parent;                                       // 재활용할 오브젝트를 모아둘 부모 오브젝트

    // T prefab: Pool에 담을 원본 오브젝트 / initialSize: 생성 개수 / parent: 부모 오브젝트
    public ObjectPool(GameObject prefab, int initialSize, Transform parent = null)
    {
        this.prefab = prefab;
        this.parent = parent;
        for(int i=0; i< initialSize; i++)
        {
            GameObject obj = GameObject.Instantiate(prefab, parent);
            obj.gameObject.SetActive(false);
            pool.Enqueue(obj);
        }
    }

    public GameObject Get()
    {
        // Pool에 해당 오브젝트가 없을 시 생성해서 반환
        GameObject obj = pool.Count > 0 ? pool.Dequeue() : GameObject.Instantiate(prefab, parent);
        obj.gameObject.SetActive(true);
        return obj;
    }

    public void Release(GameObject obj)
    {
        // 다 사용한 오브젝트는 비활성하여 pool에 반납
        obj.gameObject.SetActive(false);
        pool.Enqueue(obj);
    }
}
