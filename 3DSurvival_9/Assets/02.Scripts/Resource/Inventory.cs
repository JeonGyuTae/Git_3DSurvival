using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Inventory
{
    /// <summary>
    /// 자원을 인벤토리에 추가.
    /// 필요한 조건을 검사한 후 인벤토리에 넣는다.
    /// </summary>
    /// <returns>
    /// 성공하면 true, 실패(공간이 부족할 때 등)하면 false
    /// </returns>
    bool AddItem(ResourceData data, int amount);
}