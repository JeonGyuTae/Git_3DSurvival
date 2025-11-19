using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class AnimalConditionHandler : MonoBehaviour
{
    [SerializeField] private float health;
    public float Health {  get { return health; } }

    private Animal animal;
    private AIController controller;

    private void Start()
    {
        animal = GetComponent<Animal>();
        controller = GetComponent<AIController>();
    }

    public void SetHealth(float health)
    {
        this.health = health;
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
    }
}
