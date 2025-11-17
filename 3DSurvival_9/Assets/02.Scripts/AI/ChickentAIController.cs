using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// 닭 행동 AI Behavior
/// 
///                                       <Selector(Root)>
///             
///            <Sequence(RunAway)>                               <Sequence(Patrol)>         
/// <Condition(피격)>       <Action(RunAway)>       <Action(SetPosition)>       <Action(Move)> 
/// 
/// </summary>
public class ChickentAIController : AIController
{
    private bool isMovingToTarget = false; // 플래그 설정

    protected override void Start()
    {
        // BehaviorTree 설정
        SetBehavior();

        ResetDecisionStartTime();

        currentDestination = transform.position;
    }

    protected override void SetBehavior()
    {
        // Leaf Node 생성
        Action setDecisionNextPosition = new Action("Decision Next Position", SetDecisionPosition);
        Action move = new Action("Move Next Position", Move);

        // Sequence Node 생성
        Sequence patrol = new Sequence("Patrol Around", setDecisionNextPosition, move);

        tree = new BehaviorTree(patrol);
    }

    private NodeState SetDecisionPosition()
    {
        // targetDestination이 지정이 되었다면 Move로 이동
        if (targetDestination != Vector3.zero)
        {
            return NodeState.SUCCESS;
        }

        if (decisionStartTime < 0f)
        {
            Debug.Log("목표 설정 시작");
            // 처음 노드 진입 시 초기화
            targetDestination = Vector3.zero;
            decisionStartTime = Time.time;
            return NodeState.RUNNING;
        }

        if (Time.time < decisionStartTime + decisionDuration)
        {
            // 고민 중인 상태 일때는 RUNNING 반환
            //Debug.Log("다음 포지션 고민 중...");
            return NodeState.RUNNING;
        }
        else
        {
            Vector3 randomDirection = Random.onUnitSphere * Random.Range(minMoveDistance, maxMoveDistance);
            Vector3 randomPosition = transform.position + randomDirection;

            // 목표 설정
            if (NavMesh.SamplePosition(randomPosition, out NavMeshHit hit, maxMoveDistance, NavMesh.AllAreas))
            {
                // 목표가 탐지되면 targetDestination 설정
                targetDestination = hit.position;
                Debug.DrawRay(targetDestination, Vector3.up * 5f, Color.green, 1f);
                Debug.Log("목표 위치:" + targetDestination);

                agent.SetDestination(targetDestination);
                agent.isStopped = false;

                isMovingToTarget = true; // 플래그 설정

                // 타이머 리셋
                ResetDecisionStartTime();
                return NodeState.SUCCESS;
            }
            else
            {
                Debug.Log("탐지 못함!");
            }

            // 목표가 설정되지 못하면 다시 고민하고 목표 찾기
            ResetDecisionStartTime();
            return NodeState.RUNNING;
        }
    }

    private NodeState Move()
    {
        // 목표가 설정되지 않으면 return
        if (!agent.enabled || targetDestination == Vector3.zero || !isMovingToTarget) return NodeState.SUCCESS;


        // 이동 시작
        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            agent.isStopped = true;
            targetDestination = Vector3.zero;
            isMovingToTarget = false;
            Debug.Log("목표지점 도달");
            return NodeState.SUCCESS;
        }
        else
        {
            return NodeState.RUNNING;
        }
    }

    protected override void Update()
    {
        base.Update();
    }
}
