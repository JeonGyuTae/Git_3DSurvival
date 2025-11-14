using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Damageble
{
    void TakeDamage(int damage);
}

public class PlayerCondition : MonoBehaviour, Damageble
{
    private PlayerController controller;

    public void TakeDamage(int damage)
    {
        
    }


    void Update()
    {
        
    }
}
