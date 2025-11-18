using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Inventory
{
    /// <summary>
    /// �ڿ� �������� �κ��丮�� �߰�.
    /// ���� ������ �κ��丮 ����� �Ѵ�.
    /// </summary>
    /// <returns>�����ϸ� true, ����(���� �� ��)�� false</returns>
    bool AddItem(ResourceData data, int amount);
}