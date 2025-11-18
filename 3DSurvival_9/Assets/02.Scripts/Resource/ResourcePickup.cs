using UnityEngine;

/// <summary>
/// 바닥에 놓여 있는 자원 오브젝트
/// 팀 공통 E키 상호작용 시스템에서 Interact()를 호출해 주면
/// 자원 획득 처리 후 자기 자신을 제거한다
/// </summary>
public class ResourcePickup : MonoBehaviour
{
    [Header("Resource")]
    public ResourceData resourceData;
    public int amount = 1;

    [Header("Inventory (Optional)")]
    public MonoBehaviour inventoryBehaviour;

    private Inventory _inventory;

    private void Awake()
    {
        if (inventoryBehaviour != null)
        {
            _inventory = inventoryBehaviour as Inventory;
        }
    }

    // 팀의 E키 상호작용 시스템에서 이 함수를 호출해 주면 됨.
    // 인터페이스에 맞춰 이름만 나중에 수정 가능.
    public void Interact()
    {
        if (resourceData == null)
        {
            Debug.LogWarning($"{name} : ResourceData가 비어 있습니다.");
            return;
        }

        if (_inventory == null)
        {
            // 인벤토리 시스템 붙기 전까지는 이 로그만 나와도 충분함
            Debug.Log($"[ResourcePickup] {resourceData.displayName} x{amount} 획득 (인벤토리 미연결)");
        }
        else
        {
            bool success = _inventory.AddItem(resourceData, amount);
            if (!success)
            {
                Debug.Log($"[ResourcePickup] 인벤토리 공간 부족: {resourceData.displayName}");
                return;
            }
        }

        // 효과음/파티클 있으면 여기에서 재생
        Destroy(gameObject);   // gameObject.SetActive(false);
    }
}
