using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageble
{
    void TakeDamage(int damage);
}

public class PlayerCondition : MonoBehaviour, IDamageble
{
    private PlayerController controller;
    public UICondition uiCondition;

    Condition health { get { return uiCondition.health; } }
    Condition hunger { get { return uiCondition.hunger; } }
    Condition thirsty { get { return uiCondition.thirsty; } }
    Condition stamina { get { return uiCondition.stamina; } }

    // 배고픔과 목마름이 0일 때 체력 소모
    // 플레이어가 공격등의 데미지를 입었을 때 표현할 데미지 이벤트 액션
    private void Awake()
    {
        controller = GetComponent<PlayerController>();
    }

    void Update()
    {
        stamina.Add(stamina.passiveValue * Time.deltaTime);

        if (controller != null && controller.isRunning)
        {
            stamina.Sub(controller.useRunStamina * Time.deltaTime);
            if (stamina.curValue <= 0)
            {
                controller.isRunningFalse();
            }
        }
        if(health.curValue == 0f)
        {
            Die();
        }
    }

    void Heal(float amount)
    {
        health.Add(amount);
    }

    void Eat(float amount)
    {
        hunger.Add(amount);
    }

    void Drink(float amount)
    {
        thirsty.Add(amount);
    }

    void Rest(float amount)
    {
        stamina.Add(amount);
    }

    void Die()
    {
        // 죽는 것과 관련한 코드
    }

    public void TakeDamage(int damage)
    {
        health.Sub(damage);
        //OnTakeDamage?.Invoke();
    }

    public bool UseStamina(float amount)
    {
        if(stamina.curValue - amount < 0f)
        {
            return false;
        }

        stamina.Sub(amount);
        return true;
    }
}
