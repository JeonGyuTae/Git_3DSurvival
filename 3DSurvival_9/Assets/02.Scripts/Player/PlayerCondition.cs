using UnityEngine;

public class PlayerCondition : MonoBehaviour, IDamageable
{
    private PlayerController controller;
    public UICondition uiCondition;

    Condition health { get { return uiCondition.health; } }
    Condition hunger { get { return uiCondition.hunger; } }
    Condition thirsty { get { return uiCondition.thirsty; } }
    Condition stamina { get { return uiCondition.stamina; } }

    [SerializeField] private float noHungerHealthDecay;
    [SerializeField] private float noThirstyHealthDecay;

    public event System.Action OnTakeDamageToHalf;
    public event System.Action OnTakeDamageToZero;
    public event System.Action OnHeal;

    private bool isDead = false;

    private void Awake()
    {
        controller = GetComponent<PlayerController>();
    }

    private void Start()
    {
        
    }
    void Update()
    {
        if (isDead)
        {
            return;
        }

        hunger.Sub(hunger.passiveValue * Time.deltaTime);
        thirsty.Sub(thirsty.passiveValue * Time.deltaTime);
        stamina.Add(stamina.passiveValue * Time.deltaTime);

        // 배고픔이 0일 때 시간동안 체력 피해
        if (hunger.curValue <= 0f)
        {
            health.Sub(noHungerHealthDecay * Time.deltaTime);
        }

        // 목마름이 0일 때 시간동안 체력 피해
        if (thirsty.curValue <= 0f)
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
        if (health.curValue <= 0f)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        health.Add(amount);
        OnHeal?.Invoke();
    }

    public void Eat(float amount)
    {
        hunger.Add(amount);
        OnHeal?.Invoke();
    }

    public void Drink(float amount)
    {
        thirsty.Add(amount);
        OnHeal?.Invoke();
    }

    public void Rest(float amount)
    {
        stamina.Add(amount);
    }

    void Die()
    {
        if (isDead)
        {
            return;
        }
        isDead = true;

        if (controller != null)
        {
            controller.DisableMovement();
        }

        if (DieUIManager.Instance != null)
        {
            DieUIManager.Instance.ShowDieScreen();
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead)
        {
            return;
        }

        health.Sub(damage);
        if (health.curValue / 50 >= 1)
        {
            OnTakeDamageToHalf?.Invoke();
        }
        else
        {
            OnTakeDamageToZero?.Invoke();
        }

        if (health.curValue <= 0f)
        {
            Die();
        }
    }

    public void TakeDamage(int damage, Vector3 hitPoint)
    {
        if (isDead)
        {
            return;
        }

        health.Sub(damage);
        if (health.curValue / 50 >= 1)
        {
            OnTakeDamageToHalf?.Invoke();
        }
        else
        {
            OnTakeDamageToZero?.Invoke();
        }

        if (health.curValue <= 0f)
        {
            Die();
        }
    }

    public bool UseStamina(float amount)
    {
        if (stamina.curValue - amount < 0f)
        {
            return false;
        }

        stamina.Sub(amount);
        return true;
    }

    public void Respawn()
    {
        isDead = false;
        health.ResetToStartValue();
        hunger.ResetToStartValue();
        thirsty.ResetToStartValue();
        stamina.ResetToStartValue();

        if (controller != null)
        {
            controller.EnableMovement();
        }

        if(SpawnManager.Instance != null)
        {
            SpawnManager.Instance.TeleportToSpawnPoint();
        }
    }
}
