using UnityEngine;

[System.Serializable]
public class BuildingOption
{
    public string displayName;      // UI에서 구분용 이름 (안 써도 됨)
    public GameObject prefab;       // 실제 건물 프리팹
    public GameObject previewPrefab; // 프리뷰 프리팹
}

public class BuildingUIManager : MonoBehaviour
{
    [Header("Refs")]
    public GameObject buildUI;             // Canvas (BuildUI)
    public BuildingPlacer buildingPlacer;  // BuildingSystem에 붙은 스크립트

    [Header("Building List")]
    public BuildingOption[] buildingOptions; // 건축 옵션들 (최대 5개 넣으면 됨)

    void Start()
    {
        if (buildUI != null)
            buildUI.SetActive(false);

        // 평소엔 조준점 모드
        SetCursorLocked(true);
    }

    void Update()
    {
        // Tab 키로 건축 UI 열기/닫기
        if (Input.GetKeyDown(KeyCode.T))
        {
            ToggleUI();
        }
    }

    // 커서 잠금/해제
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
            // UI 열릴 때: 배치 모드 종료 + 마우스 보이게
            if (buildingPlacer != null)
                buildingPlacer.CancelPlacing();

            SetCursorLocked(false);
        }
        else
        {
            // UI 닫으면 다시 조준점 모드
            SetCursorLocked(true);
        }
    }

    // 버튼에서 index를 넘겨서 호출할 함수
    public void SelectBuilding(int index)
    {
        if (buildingOptions == null || index < 0 || index >= buildingOptions.Length)
        {
            Debug.LogWarning("BuildingUIManager: 잘못된 building index: " + index);
            return;
        }

        BuildingOption opt = buildingOptions[index];

        if (opt.prefab == null || opt.previewPrefab == null)
        {
            Debug.LogWarning("BuildingUIManager: prefab 또는 previewPrefab 이 비어 있습니다. index = " + index);
            return;
        }

        Debug.Log("SelectBuilding: " + opt.displayName);

        // UI 닫고 조준점 모드로 전환
        if (buildUI != null)
            buildUI.SetActive(false);

        SetCursorLocked(true);

        if (buildingPlacer != null)
            buildingPlacer.StartPlacing(opt.prefab, opt.previewPrefab);
    }
}
