using UnityEngine;

public class BuildingPlacer : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] private Camera playerCamera;   // 비워두면 Update에서 Camera.main으로 자동 설정

    [Header("Preview Visual")]
    [SerializeField] private Material validMaterial;    // 설치 가능일 때 색 (초록)
    [SerializeField] private Material invalidMaterial;  // 설치 불가일 때 색 (빨강)

    [Header("Raycast Settings")]
    [SerializeField] private LayerMask placeMask;       // 땅, 설치 가능한 레이어들

    [Header("Rotation")]
    [SerializeField] private float rotateStep = 15f;    // R 키 한 번 누를 때 회전 각도
    private float currentYRotation = 0f;
    private float maxPlaceDistance = 5f;
    private Quaternion baseRotation = Quaternion.identity;

    private GameObject buildingPrefab;      // 실제 설치될 프리팹
    private GameObject previewPrefab;       // 프리뷰용 프리팹
    private GameObject currentPreview;      // 현재 씬에 떠 있는 프리뷰 오브젝트
    private Renderer[] previewRenderers;    // 프리뷰에 포함된 Renderer들

    private bool isPlacing = false;         // 건축 모드 on/off
    private bool canPlace = false;          // 현재 위치에 설치 가능한지 여부

    /// <summary>
    /// UI에서 건축 버튼을 클릭했을 때 호출.
    /// </summary>
    public void StartPlacing(GameObject build, GameObject preview)
    {
        buildingPrefab = build;
        previewPrefab = preview;

        if (buildingPrefab == null || previewPrefab == null)
        {
            Debug.LogWarning("BuildingPlacer: buildingPrefab 또는 previewPrefab 이 비었습니다.");
            return;
        }

        // 기존 프리뷰가 남아있다면 정리
        if (currentPreview != null)
        {
            Destroy(currentPreview);
        }

        isPlacing = true;

        // 프리뷰 생성
        currentPreview = Instantiate(previewPrefab);
        previewRenderers = currentPreview.GetComponentsInChildren<Renderer>();

        // 혹시 남아 있을지 모르는 물리/콜라이더는 안전하게 비활성화
        foreach (var rb in currentPreview.GetComponentsInChildren<Rigidbody>())
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        foreach (var col in currentPreview.GetComponentsInChildren<Collider>())
        {
            col.enabled = false;   // 프리뷰는 충돌 X
        }

        // 회전값 초기화
        currentYRotation = 0f;
        baseRotation = currentPreview.transform.rotation;
        ApplyRotation();

        canPlace = false;
        UpdatePreviewColor();
    }

    /// <summary>
    /// 건축 모드 강제 종료(프리뷰 삭제).
    /// </summary>
    public void CancelPlacing()
    {
        isPlacing = false;
        if (currentPreview != null)
        {
            Destroy(currentPreview);
            currentPreview = null;
        }
    }

    private void Update()
    {
        if (!isPlacing)
            return;

        // 1. 카메라가 아직 설정되지 않았다면 계속 MainCamera를 찾는다.
        if (playerCamera == null)
        {
            playerCamera = Camera.main;
            if (playerCamera == null)
            {
                // 아직 Player가 스폰되지 않았으면 그냥 대기
                return;
            }
        }

        // 2. 프리뷰 위치/색 업데이트
        UpdatePreviewPosition();

        // 3. 회전 처리 (R 키)
        HandleRotationInput();

        // 4. 설치 / 취소 입력 처리
        HandlePlaceInput();
    }

    /// <summary>
    /// 화면 중앙(조준점)에서 레이 쏴서 프리뷰 위치 업데이트.
    /// </summary>
    private void UpdatePreviewPosition()
    {
        if (currentPreview == null)
            return;

        Vector3 screenCenter = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0f);
        Ray ray = playerCamera.ScreenPointToRay(screenCenter);

        if (Physics.Raycast(ray, out RaycastHit hit, maxPlaceDistance, placeMask))
        {
            currentPreview.transform.position = hit.point;
            canPlace = true;
        }
        else
        {
            // 땅을 못 맞추면 설치 불가 상태로만 표시
            canPlace = false;
        }

        UpdatePreviewColor();
    }

    /// <summary>
    /// R 키 입력으로 Y축 회전.
    /// </summary>
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

    /// <summary>
    /// 좌클릭으로 설치, ESC로 취소.
    /// </summary>
    private void HandlePlaceInput()
    {
        // 설치 취소
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CancelPlacing();
            return;
        }

        // 설치 시도 (좌클릭)
        if (Input.GetMouseButtonDown(0))
        {
            if (!canPlace || currentPreview == null || buildingPrefab == null)
                return;

            // 실제 건물 설치
            Instantiate(
                buildingPrefab,
                currentPreview.transform.position,
                currentPreview.transform.rotation
            );

            // 한 번 설치 후 건축 모드 종료 (원하면 유지해도 됨)
            CancelPlacing();
        }
    }

    /// <summary>
    /// 설치 가능/불가에 따라 프리뷰 색 변경.
    /// </summary>
    private void UpdatePreviewColor()
    {
        if (previewRenderers == null)
            return;

        Material targetMat = canPlace ? validMaterial : invalidMaterial;

        foreach (var rend in previewRenderers)
        {
            // 프리뷰 전용이라 sharedMaterial 사용해도 OK
            rend.sharedMaterial = targetMat;
        }
    }
}
