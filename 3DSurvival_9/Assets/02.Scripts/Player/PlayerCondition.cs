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

    [SerializeField] private float noHungerHealthDecay;
    [SerializeField] private float noThirstyHealthDecay;
    // 배고픔과 목마름이 0일 때 체력 소모
    // 플레이어가 공격 등의 데미지를 입었을 때 표현할 데미지 이벤트 액션
    private void Awake()
    {
        controller = GetComponent<PlayerController>();
    }

    void Update()
    {
        hunger.Sub(hunger.passiveValue * Time.deltaTime);
        thirsty.Sub(thirsty.passiveValue * Time.deltaTime);
        stamina.Add(stamina.passiveValue * Time.deltaTime);

        // 배고픔이 0일 때 시간동안 체력 피해
        if(hunger.curValue <= 0f)
        {
            health.Sub(noHungerHealthDecay * Time.deltaTime);
        }

        // 목마름이 0일 때 시간동안 체력 피해
        if(thirsty.curValue <= 0f)
        {
            health.Sub(noThirstyHealthDecay * Time.deltaTime);
        }

        // 달리기도중 스태미나가 0이되면 달리기 상태를 해제
        if (controller != null && controller.isRunning)
        {
            stamina.Sub(controller.useRunStamina * Time.deltaTime);
            if (stamina.curValue <= 0)
            {
                controller.runInputHoldFalse();
            }
        }

        // 체력이 0이 됐을 때
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
        Debug.Log("주금");
        // 플레이어 죽음 코드
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
