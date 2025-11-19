using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalConditionHandler : MonoBehaviour
{
    [SerializeField] private float health;
    public float Health {  get { return health; } }

    private Animal animal;

    private void Start()
    {
        animal = GetComponent<Animal>();
    }

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


        // AnimalSpawnManager에게 자신이 죽었음을 알림
        string key = animal.Data.animalName;
        GameObject obj = this.gameObject;

        AnimalSpawnManager.Instance.Release(key, obj);
    }
}
