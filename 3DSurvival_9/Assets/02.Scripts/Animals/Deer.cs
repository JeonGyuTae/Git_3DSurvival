using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Deer 스크립트
/// 사슴에 대한 Interact를 처리하는 스크립트이다.
/// </summary>
public class Deer : MonoBehaviour, IInteractable
{
    [SerializeField] private AnimalData data;

    private HerbivoreAIController controller;

    private void Awake()
    {
        controller = GetComponent<HerbivoreAIController>();
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
