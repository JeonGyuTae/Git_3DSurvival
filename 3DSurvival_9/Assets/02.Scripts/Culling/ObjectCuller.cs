using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 오브젝트 컬링 스크립트
/// 
/// Unity Collider TriggerEnter를 이용
/// Layer 마스크 기준으로 오브젝트 컬링 한다.
/// </summary>
public class ObjectCuller : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        ICullable cullable = other.GetComponentInParent<ICullable>();
        if(cullable != null)
        {
            cullable.EnableCullComponents();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        ICullable cullable = other.GetComponentInParent<ICullable>();
        if (cullable != null)
        {
            cullable.DisableCullComponents();
        }
    }
}
