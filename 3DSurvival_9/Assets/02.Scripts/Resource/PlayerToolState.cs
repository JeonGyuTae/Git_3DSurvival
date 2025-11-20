using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerToolState : MonoBehaviour
{
    [Header("현재 들고 있는 도구 타입")]
    public ToolType currentTool = ToolType.None;

    // 나중에 인벤토리/장비 시스템에서
    // 아이템 장착 시 이 값을 Axe / Hammer로 바꿔주면 됨
}

