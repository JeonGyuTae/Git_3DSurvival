using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boat : MonoBehaviour, IInteractable
{
    private EndingUI endingUI;
    private string endingTxt = "The End";

    private void Start()
    {
        endingUI = FindFirstObjectByType<EndingUI>();
    }

    public InteractableType GetInteractableType()
    {
        return InteractableType.NPC;
    }

    public string GetInteractPrompt()
    {
        return endingTxt;
    }

    public void HideInteractUI()
    {

    }

    public void ShowInteractUI()
    {

    }

    public void OnInteract()
    {
        endingUI.gameObject.SetActive(true);
    }
}
