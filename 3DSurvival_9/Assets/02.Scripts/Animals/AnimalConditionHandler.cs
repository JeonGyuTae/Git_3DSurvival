using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalConditionHandler : MonoBehaviour
{
    Condition health { get { return health; } }

    public void TakeDamage(int damage)
    {
        health.Sub(damage);
        CheckHealth();
    }

    private void CheckHealth()
    {
        if (health.curValue <= 0)
            Die();
    }

    public void Die()
    {
        Debug.Log("주금");
        // 플레이어 죽음 코드
    }
}
