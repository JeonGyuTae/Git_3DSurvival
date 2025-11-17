using UnityEngine;

public class BuildingPlacer : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] private Camera playerCamera;   // 조준에 사용할 카메라 (필수로 넣기!)

    [Header("Preview Visual")]
    public Material validMaterial;      // 설치 가능(초록)
    public Material invalidMaterial;    // 설치 불가(빨강)

    [Header("Raycast Settings")]
    public LayerMask placeMask;         // Ground, Terrain 등 (비어 있으면 전체)

    private GameObject buildingPrefab;  // 실제 건물
    private GameObject previewPrefab;   // 프리뷰

    private GameObject currentPreview;
    private Renderer[] previewRenderers;

    private bool isPlacing = false;
    private bool canPlace = false;

    void Update()
    {
        if (!isPlacing || currentPreview == null)
            return;

        UpdatePreviewPosition();

        if (Input.GetMouseButtonDown(0))
        {
            PlaceBuilding();
        }

        if (Input.GetMouseButtonDown(1))
        {
            CancelPlacing();
        }
    }

    // UI에서 건물 선택 시 호출
    public void StartPlacing(GameObject build, GameObject preview)
    {
        buildingPrefab = build;
        previewPrefab = preview;

        if (buildingPrefab == null || previewPrefab == null)
        {
            Debug.LogWarning("BuildingPlacer: buildingPrefab / previewPrefab 이 비어 있습니다.");
            return;
        }

        if (currentPreview != null)
            Destroy(currentPreview);

        isPlacing = true;

        currentPreview = Instantiate(previewPrefab);
        previewRenderers = currentPreview.GetComponentsInChildren<Renderer>();

        canPlace = false;
        UpdatePreviewColor();
    }

    // 화면 중앙(조준점) 기준으로 Ray 쏴서 프리뷰 위치 갱신
    void UpdatePreviewPosition()
    {
        if (playerCamera == null)
        {
            Debug.LogWarning("BuildingPlacer: playerCamera 가 비어 있습니다. 인스펙터에서 카메라를 지정하세요.");
            return;
        }

        // 화면 정중앙 좌표 (조준점)
        Vector3 center = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);
        Ray ray = playerCamera.ScreenPointToRay(center);
        RaycastHit hit;

        // placeMask 가 비어 있으면 전체 대상으로 Raycast
        int mask = placeMask.value == 0 ? ~0 : placeMask.value;

        if (Physics.Raycast(ray, out hit, 500f, mask))
        {
            currentPreview.transform.position = hit.point;
            canPlace = true;
        }
        else
        {
            canPlace = false;
        }

        UpdatePreviewColor();
    }

    void UpdatePreviewColor()
    {
        if (previewRenderers == null) return;
        if (validMaterial == null || invalidMaterial == null) return;

        Material target = canPlace ? validMaterial : invalidMaterial;

        foreach (var rend in previewRenderers)
        {
            rend.sharedMaterial = target;
        }
    }

    void PlaceBuilding()
    {
        if (!canPlace || buildingPrefab == null || currentPreview == null)
            return;

        Instantiate(
            buildingPrefab,
            currentPreview.transform.position,
            currentPreview.transform.rotation
        );
    }

    public void CancelPlacing()
    {
        isPlacing = false;
        canPlace = false;

        if (currentPreview != null)
        {
            Destroy(currentPreview);
            currentPreview = null;
        }

        previewRenderers = null;
    }
}
