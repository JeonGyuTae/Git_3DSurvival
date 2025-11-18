using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tiger : MonoBehaviour, IInteractable
{
    [SerializeField] private AnimalData data;

    private CarnivoreAIController controller;

    private void Awake()
    {
        controller = GetComponent<CarnivoreAIController>();
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
