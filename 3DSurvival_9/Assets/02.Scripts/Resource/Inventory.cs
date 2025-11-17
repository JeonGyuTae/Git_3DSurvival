using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Inventory
{
    /// <summary>
    /// 자원 아이템을 인벤토리에 추가.
    /// 실제 구현은 인벤토리 담당이 한다.
    /// </summary>
    /// <returns>성공하면 true, 실패(가득 참 등)면 false</returns>
    bool AddItem(ResourceData data, int amount);
}