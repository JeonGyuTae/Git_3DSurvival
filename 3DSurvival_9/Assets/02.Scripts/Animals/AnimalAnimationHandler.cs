using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;


/// <summary>
/// Animal 애니메이션 핸들러 스크립트
/// Asset에 BlendTree의 값을 조절하며 애니메이션을 적용한다.
/// </summary>
public class AnimalAnimationHandler : MonoBehaviour
{
    private Animator _animator;
    private readonly string VerticalID = "Vert";
    private readonly string StateID = "State";

    private float flowState;
    private Vector2 flowAxis;
    private readonly float inputFlow = 4.5f;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void Animate(in Vector2 axis, float state, float deltaTime)
    {
        // m_FlowAxis를 입력 axis로 부드럽게 보간
        Vector2 deltaFlowAxis = axis - flowAxis;

        // 부드러운 유도 흐름 계산
        flowAxis = Vector2.ClampMagnitude(flowAxis + inputFlow * deltaTime * deltaFlowAxis, 1f);

        // m_FlowState를 입력 state로 부드럽게 보간 (0 또는 1)
        float deltaFlowState = state - flowState;
        flowState = Mathf.Clamp01(flowState + inputFlow * deltaTime * Mathf.Sign(deltaFlowState));

        // 애니메이터에 값 설정
        _animator.SetFloat(VerticalID, flowAxis.magnitude);
        _animator.SetFloat(StateID, flowState); 
    }
}
