using UnityEngine;

public class MapController : MonoBehaviour
{
    public GameObject map;
    public GameObject mMap;

    void Awake()
    {
        if (map == null)
        {
            Debug.Log("Map이 연동되지 않음");
        }

        if (mMap == null)
        {
            Debug.Log("Map이 연동되지 않음");
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            OnMap();
        }
    }

    void OnMap()
    {
        // 현재 상태 반전_지도(토글)
        bool isMapActive = map.activeSelf;
        map.SetActive(!isMapActive);

        // 현재 상태 반전_미니맵(토글)
        bool isMMapActive = mMap.activeSelf;
        mMap.SetActive(!isMMapActive);
    }
}
