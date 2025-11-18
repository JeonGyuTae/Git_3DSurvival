using UnityEngine;

public class BuildingPlacer : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] private Camera playerCamera;   // 플레이어 카메라

    [Header("Preview Visual")]
    public Material validMaterial;      // 설치 가능(초록)
    public Material invalidMaterial;    // 설치 불가(빨강)

    [Header("Raycast Settings")]
    public LayerMask placeMask;         // Ground, Terrain 등 (없으면 전체)

    [Header("Rotation")]
    [SerializeField] private float rotationStep = 45f; // R키 한 번당 회전 각도
    private float currentYRotation = 0f;               // 현재 회전 값
    private Quaternion baseRotation;                   // 프리뷰 기본 회전값

    private GameObject buildingPrefab;  // 실제 건물 프리팹
    private GameObject previewPrefab;   // 프리뷰 프리팹

    private GameObject currentPreview;
    private Renderer[] previewRenderers;

    private bool isPlacing = false;
    private bool canPlace = false;

    void Awake()
    {
        if (playerCamera == null)
            playerCamera = Camera.main;
    }

    void Update()
    {
        if (!isPlacing || currentPreview == null)
            return;

        UpdatePreviewPosition();

        if (Input.GetKeyDown(KeyCode.R))
        {
            RotatePreview();
        }

        if (Input.GetMouseButtonDown(0))
        {
            PlaceBuilding();
        }

        if (Input.GetMouseButtonDown(1))
        {
            CancelPlacing();
        }
    }

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

        // 회전값 초기화
        baseRotation = currentPreview.transform.rotation;
        currentYRotation = 0f;
        ApplyRotation();

        canPlace = false;
        UpdatePreviewColor();
    }

    // 화면 중앙(조준점) 기준으로 레이 쏴서 프리뷰 위치 갱신
    void UpdatePreviewPosition()
    {
        if (playerCamera == null)
        {
            Debug.LogWarning("BuildingPlacer: playerCamera 가 비어 있습니다. 인스펙터에서 카메라를 지정하세요.");
            return;
        }

        Vector3 center = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);
        Ray ray = playerCamera.ScreenPointToRay(center);
        RaycastHit hit;

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
    void RotatePreview()
    {
        currentYRotation += rotationStep;
        if (currentYRotation >= 360f) currentYRotation -= 360f;

        ApplyRotation();
    }

    void ApplyRotation()
    {
        if (currentPreview == null) return;
        currentPreview.transform.rotation = baseRotation * Quaternion.Euler(0f, currentYRotation, 0f);
    }

    void PlaceBuilding()
    {
        if (!canPlace || buildingPrefab == null || currentPreview == null)
            return;

        Instantiate(
            buildingPrefab,
            currentPreview.transform.position,
            currentPreview.transform.rotation   // 프리뷰 방향 그대로 설치
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
