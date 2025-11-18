using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dog : MonoBehaviour
{
    [SerializeField] private AnimalData data;

    private CarnivoreAIController controller;
    private AnimalConditionHandler conditionHandler;

    private void Awake()
    {
        controller = GetComponent<CarnivoreAIController>();
        conditionHandler = GetComponent<AnimalConditionHandler>();

        Init();
    }

    private void Init()
    {
        // 체력 설정
        conditionHandler.SetHealth(data.maxHp);
    }

    public InteractableType GetInteractableType()
    {
        return InteractableType.Animal;
    }

    public string GetInteractPrompt()
    {
        return data.animalName;
    }

    public void ShowInteractUI()
    {

    }

    public void HideInteractUI()
    {

    }

    public void OnInteract()
    {
        // Test 공격 판정
        // Raycast로 hit 된 Position 값을 얻어와야 함

        Interaction interact = GameObject.FindAnyObjectByType<Interaction>();
        controller.OnHit(interact.hitPosition);
    }
}
