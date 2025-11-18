using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalConditionHandler : MonoBehaviour
{
    [SerializeField] private float health;
    public float Health {  get { return health; } }

    public void SetHealth(float health)
    {
        this.health = health;
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        CheckHealth();
    }

    private void CheckHealth()
    {
        if (health <= 0)
            Die();
    }

    public void Die()
    {
        Debug.Log("주금");
        // 플레이어 죽음 코드
    }
}
