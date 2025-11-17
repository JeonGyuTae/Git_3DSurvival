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

    Condition health; //{ get {  return } }
    Condition hunger;
    Condition thirsty;
    Condition stamina;

    private void Awake()
    {
        controller = GetComponent<PlayerController>();
    }

    void Update()
    {
        
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
