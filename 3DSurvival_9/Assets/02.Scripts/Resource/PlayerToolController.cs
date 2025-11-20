using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerToolController : MonoBehaviour
{
    public ToolType currentTool = ToolType.None;

    // 인벤토리 팀이 "도끼 장착"할 때 이 함수를 호출해주면 됨.
    public void EquipAxe()
    {
        currentTool = ToolType.Axe;
    }

    public void EquipPickaxe()
    {
        currentTool = ToolType.Hammer;
    }

    public void UnequipTool()
    {
        currentTool = ToolType.None;
    }
}
