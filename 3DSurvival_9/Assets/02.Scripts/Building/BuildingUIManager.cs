using UnityEngine;

#region Data classes

[System.Serializable]
public class BuildingCost
{
    public string resourceName;
    public int amount = 1;
}

[System.Serializable]
public class BuildingOption
{
    public string displayName;        // UI 표시용 이름
    public GameObject prefab;         // 실제 설치될 프리팹
    public GameObject previewPrefab;  // 프리뷰 프리팹

    public BuildingCost[] cost;       // 이 건물에 필요한 재료들
}

#endregion

public class BuildingUIManager : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private GameObject buildUI;
    [SerializeField] private BuildingPlacer buildingPlacer;

    [Header("Building List")]
    public BuildingOption[] buildingOptions;

    private bool isOpen = false;

    private void Start()
    {
        SetUI(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            ToggleUI();
        }
    }

    public void ToggleUI()
    {
        SetUI(!isOpen);
    }

    private void SetUI(bool open)
    {
        isOpen = open;

        if (buildUI != null)
            buildUI.SetActive(open);

        if (open)
        {
            if (buildingPlacer != null)
                buildingPlacer.CancelPlacing();

            SetCursorLocked(false);
        }
        else
        {

            SetCursorLocked(true);
        }
    }

    private void SetCursorLocked(bool locked)
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

        SetUI(false);

        if (buildingPlacer != null)
            buildingPlacer.StartPlacing(opt);
    }
}
