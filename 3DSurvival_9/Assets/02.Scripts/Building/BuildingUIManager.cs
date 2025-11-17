using UnityEngine;

public class BuildingUIManager : MonoBehaviour
{
    public GameObject buildUI;           // Canvas (BuildUI)
    public BuildingPlacer buildingPlacer;

    [Header("House")]
    public GameObject housePrefab;
    public GameObject housePreview;

    void Start()
    {
        if (buildUI != null)
            buildUI.SetActive(false);

        // 게임 시작할 때는 커서 잠근 상태(원래 FPS 상태)로 시작
        SetCursorLocked(true);
    }

    void Update()
    {
        // 1번으로 UI 켜고 끄기
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ToggleUI();
        }
    }

    // 커서 잠금/해제 한 곳에서 처리
    void SetCursorLocked(bool locked)
    {
        if (locked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void ToggleUI()
    {
        if (buildUI == null) return;

        bool open = !buildUI.activeSelf;
        buildUI.SetActive(open);

        if (open)
        {
            // UI 열리면 : 건축 모드는 끄고, 커서 풀기
            buildingPlacer.CancelPlacing();
            SetCursorLocked(false);   // ← 여기 중요
        }
        else
        {
            // UI 닫히면 : 다시 FPS 모드처럼 커서 잠그기
            SetCursorLocked(true);    // ← 여기 중요
        }
    }

    // 버튼에서 연결할 함수
    public void OnClick_House()
    {
        // UI 닫고 다시 게임 모드
        if (buildUI != null)
            buildUI.SetActive(false);

        SetCursorLocked(true); // 다시 중앙 고정 + 숨김 (조준점 모드)

        if (buildingPlacer != null)
            buildingPlacer.StartPlacing(housePrefab, housePreview);
    }
}
