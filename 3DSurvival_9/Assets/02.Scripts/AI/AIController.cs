using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 모든 AI가 상속받는 Controller 클래스
/// BehaviorTree 구조로 움직임을 구현하며
/// AI Navigation을 연동하여 활동 지역을 지정함
/// </summary>
public class AIController : MonoBehaviour
{
    private BehaviorTree tree;

    private void Start()
    {
        // BehaviorTree 설정
        SetBehavior();
    }

    private void SetBehavior()
    {
        // Leaf Node 생성
        Action decisionNextPosition = new Action("Decision Next Position", DecisionPosition);
        Action move = new Action("Move Next Position", Move);

        // Sequence Node 생성
        Sequence patrol = new Sequence("Patrol Around", decisionNextPosition, move);

        tree = new BehaviorTree(patrol);
    }

    private NodeState DecisionPosition()
    {
        Debug.Log("다음 포지션 고민 중...");
        return NodeState.SUCCESS;
    }

    private NodeState Move()
    {
        Debug.Log("다음 포지션으로 이동");
        return NodeState.SUCCESS;
    }

    private void Update()
    {
        // BehaviorTree 실행
        tree.RunTree();
    }
}
