using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Interactable 오브젝트 인터페이스
/// [E]키를 눌러 상호작용 할 수 있는 오브젝트에 사용
/// Raycast에 닿으면 상호작용 오브젝트에 대한 정보가 뜸(GetPrompt)
/// 상호작용 가능한 오브젝트가 있을 때 
/// </summary>
public interface IInteractable
{
    public string GetInteractPrompt();      // 상호작용 오브젝트 정보 전달
    public void ShowInteractUI();           // 오브젝트 정보 UI 활성화
    public void HideInteractUI();           // 오브젝트 정보 UI 비활성화
    public void OnInteract();               // [E]키를 눌러 상호작용 시 호출
}
