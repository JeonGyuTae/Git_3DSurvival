using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ToolAttack : MonoBehaviour
{
    [Header("References")]
    public Camera cam;
    public float attackDistance = 3f;
    public LayerMask hitMask;  // 나무/바위가 있는 레이어 (보통 Default/Interactable)

    private PlayerToolState toolState;

    private void Awake()
    {
        toolState = GetComponent<PlayerToolState>();
        if (cam == null)
        {
            cam = Camera.main;
        }
    }

    // 입력 시스템에서 공격 버튼에 연결 (예: 마우스 왼쪽 / RT)
    public void OnAttackInput(InputAction.CallbackContext context)
    {
        if (context.phase != InputActionPhase.Started)
            return;

        TryHitNode();
    }

    private void TryHitNode()
    {
        if (cam == null || toolState == null)
            return;

        Ray ray = cam.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, attackDistance, hitMask))
        {
            var node = hit.collider.GetComponentInParent<HarvestableNode>();
            if (node != null)
            {
                node.Harvest(gameObject, toolState.currentTool);
            }
        }
    }
}

