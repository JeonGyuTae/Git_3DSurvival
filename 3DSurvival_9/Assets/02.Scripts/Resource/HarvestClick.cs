using UnityEngine;

public class HarvestClick : MonoBehaviour
{
    [Header("Raycast")]
    public Camera cam;              // 플레이어 카메라
    public float distance = 3f;     // 캐기 거리
    public LayerMask harvestMask;   // 나무/바위 레이어

    private void Update()
    {
        // 마우스 좌클릭
        if (Input.GetMouseButtonDown(0))
        {
            TryHarvest();
        }
    }

    private void TryHarvest()
    {
        if (cam == null)
        {
            Debug.LogWarning("HarvestClick: cam이 비어 있음");
            return;
        }

        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, distance, harvestMask))
        {
            HarvestableNode node = hit.collider.GetComponentInParent<HarvestableNode>();
            if (node != null)
            {
                Debug.Log($"[HarvestClick] {hit.collider.name} 에 Harvest 호출");
                node.Harvest();
            }
        }
    }
}
