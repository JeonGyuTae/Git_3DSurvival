using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Chicken 스크립트
/// 닭에 대한 Interact를 처리하는 스크립트이다.
/// </summary>
public class Chicken : MonoBehaviour, IInteractable
{
    [SerializeField] private AnimalData data;

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

    }
}
