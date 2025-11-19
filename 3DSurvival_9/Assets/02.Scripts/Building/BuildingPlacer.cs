using UnityEngine;

public class BuildingPlacer : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] private Camera playerCamera;           // 비워두면 Camera.main 사용

    [Header("Preview Visual")]
    [SerializeField] private Material validMaterial;        // 설치 가능 (초록)
    [SerializeField] private Material invalidMaterial;      // 설치 불가 (빨강)

    [Header("Raycast Settings")]
    [SerializeField] private LayerMask placeMask;           // Ground 등
    [SerializeField] private float maxPlaceDistance = 6f;   // 설치 최대 거리

    [Header("Inventory")]
    [SerializeField] private PlayerInventory playerInventory; // 인벤토리 (비워둬도 자동 찾음)

    [Header("Rotation")]
    [SerializeField] private float rotateStep = 15f;

    // 현재 선택된 건물 옵션
    private BuildingOption currentOption;

    // 프리팹 관련
    private GameObject buildingPrefab;
    private GameObject previewPrefab;
    private GameObject currentPreview;
    private Renderer[] previewRenderers;

    private bool isPlacing = false;
    private bool canPlace = false;

    private float currentYRotation = 0f;
    private Quaternion baseRotation = Quaternion.identity;

    // ──────────────────────────────────────────────
    // PlayerInventory 자동 찾기
    // ──────────────────────────────────────────────
    private void TryGetInventory()
    {
        if (playerInventory == null)
        {
            playerInventory = FindObjectOfType<PlayerInventory>();
            if (playerInventory != null)
            {
                Debug.Log("BuildingPlacer: PlayerInventory 자동 연결됨.");
            }
        }
    }

    /// <summary>
    /// UI에서 건축 버튼을 눌렀을 때 호출.
    /// </summary>
    public void StartPlacing(BuildingOption option)
    {
        if (option == null)
            return;

        currentOption = option;
        buildingPrefab = option.prefab;
        previewPrefab = option.previewPrefab;

        if (buildingPrefab == null || previewPrefab == null)
        {
            Debug.LogWarning("BuildingPlacer: prefab 또는 previewPrefab 이 비어 있습니다.");
            return;
        }

        // 기존 프리뷰 정리
        if (currentPreview != null)
        {
            Destroy(currentPreview);
        }

        isPlacing = true;

        // 프리뷰 생성
        currentPreview = Instantiate(previewPrefab);
        previewRenderers = currentPreview.GetComponentsInChildren<Renderer>();

        // 프리뷰는 물리/콜라이더 꺼두기
        foreach (var rb in currentPreview.GetComponentsInChildren<Rigidbody>())
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        foreach (var col in currentPreview.GetComponentsInChildren<Collider>())
        {
            col.enabled = false;
        }

        currentYRotation = 0f;
        baseRotation = currentPreview.transform.rotation;
        ApplyRotation();

        canPlace = false;
        UpdatePreviewColor();
    }

    /// <summary>
    /// 건축 모드 종료 (프리뷰 삭제)
    /// </summary>
    public void CancelPlacing()
    {
        isPlacing = false;
        currentOption = null;
        buildingPrefab = null;
        previewPrefab = null;

        if (currentPreview != null)
        {
            Destroy(currentPreview);
            currentPreview = null;
        }
    }

    private void Update()
    {
        // 먼저 인벤토리 자동 연결 시도
        TryGetInventory();

        if (!isPlacing)
            return;

        // 카메라 자동 할당
        if (playerCamera == null)
        {
            playerCamera = Camera.main;
            if (playerCamera == null)
                return;
        }

        UpdatePreviewPosition();
        HandleRotationInput();
        HandlePlaceInput();
    }

    private void UpdatePreviewPosition()
    {
        if (currentPreview == null)
            return;

        Vector3 screenCenter = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0f);
        Ray ray = playerCamera.ScreenPointToRay(screenCenter);

        bool groundOk = false;

        if (Physics.Raycast(ray, out RaycastHit hit, maxPlaceDistance, placeMask))
        {
            currentPreview.transform.position = hit.point;

            // 경사 각도 체크 (너무 가파르면 설치 불가)
            float angle = Vector3.Angle(hit.normal, Vector3.up);
            const float maxSlope = 50f;
            groundOk = angle <= maxSlope;
        }
        else
        {
            groundOk = false;
        }

        // 재료 조건 체크
        bool resourceOk = HasRequiredResources();

        canPlace = groundOk && resourceOk;
        UpdatePreviewColor();
    }

    private void HandleRotationInput()
    {
        if (currentPreview == null)
            return;

        if (Input.GetKeyDown(KeyCode.R))
        {
            currentYRotation += rotateStep;
            ApplyRotation();
        }
    }

    private void ApplyRotation()
    {
        if (currentPreview == null)
            return;

        currentPreview.transform.rotation =
            baseRotation * Quaternion.Euler(0f, currentYRotation, 0f);
    }

    private void HandlePlaceInput()
    {
        // ESC로 취소
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CancelPlacing();
            return;
        }

        // 좌클릭으로 설치 시도
        if (Input.GetMouseButtonDown(0))
        {
            if (!canPlace || currentPreview == null || buildingPrefab == null)
                return;

            // 혹시 중간에 인벤에서 버렸을 수도 있으니 한 번 더 체크
            if (!HasRequiredResources())
            {
                Debug.Log("건축 실패: 재료가 부족합니다.");
                return;
            }

            // 실제 건물 설치 (※ 지금은 재료를 차감하지는 않음)
            Instantiate(
                buildingPrefab,
                currentPreview.transform.position,
                currentPreview.transform.rotation
            );

            // 한 번 설치 후 배치 모드 종료
            CancelPlacing();
        }
    }

    private void UpdatePreviewColor()
    {
        if (previewRenderers == null)
            return;

        Material targetMat = canPlace ? validMaterial : invalidMaterial;

        foreach (var rend in previewRenderers)
        {
            rend.sharedMaterial = targetMat;
        }
    }

    private bool HasRequiredResources()
    {
        if (playerInventory == null || currentOption == null ||
            currentOption.cost == null || currentOption.cost.Length == 0)
            return true;

        var slots = playerInventory.slots;
        if (slots == null) return true;

        foreach (var c in currentOption.cost)
        {
            if (c == null || string.IsNullOrEmpty(c.resourceName) || c.amount <= 0)
                continue;

            int total = 0;

            for (int i = 0; i < slots.Length; i++)
            {
                var data = slots[i].itemdata;
                if (data == null) continue;

                if (data.itemname == c.resourceName)
                {
                    total += slots[i].quantity;
                }
            }

            // 하나라도 부족하면 실패
            if (total < c.amount)
                return false;
        }

        // 모든 재료가 충분
        return true;
    }
}
