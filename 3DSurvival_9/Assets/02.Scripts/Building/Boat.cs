using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boat : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject endingUI;
    private string endingTxt = "The End";

    private void Start()
    {
        endingUI = GameObject.Find("EndingUI");
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
        endingUI.transform.GetChild(0).gameObject.SetActive(true);
    }
}
