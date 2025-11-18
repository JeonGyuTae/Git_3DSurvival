using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.LightAnchor;

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
    private AnimalAnimationHandler animationHandler;

    private Vector3 hitPosition;
    private const float OFFSET_RUN_ANGLE = 5.0f;

    [SerializeField] private bool isHit = false;
    public bool IsHit { get { return isHit; } set { isHit = value; } }

    protected override void Awake()
    {
        base.Awake();
        animationHandler = GetComponent<AnimalAnimationHandler>();
    }

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
        Action setPatrolTargetPosition = new Action("Decision Next Position", SetPatrolTargetPosition);
        Action moveToTarget = new Action("Move Next Position", MoveToTarget);
        ConditionNode checkDamage = new ConditionNode("Check Damage From Player", CheckDamage);
        Action hit = new Action("Hit", Hit);
        Action setRunTargetPosition = new Action("Set Run Position", SetRunTargetPosition);

        // Sequence Patrol 생성
        Sequence patrol = new Sequence("Patrol Around", setPatrolTargetPosition, moveToTarget);

        // Sequence RunAway 생성
        Sequence runAway = new Sequence("RunAway", checkDamage, hit, setRunTargetPosition, moveToTarget);

        // Selector Root 생성
        Selector root = new Selector("Chicken Root", runAway, patrol);

        tree = new BehaviorTree(root);
    }

    #region Sequence : Patrol

    private NodeState SetPatrolTargetPosition()
    {
        // targetDestination이 지정이 되었다면 Move로 이동
        if (targetDestination != Vector3.zero)
        {
            return NodeState.SUCCESS;
        }

        if (decisionStartTime < 0f)
        {
            // 처음 노드 진입 시 초기화
            targetDestination = Vector3.zero;
            decisionStartTime = Time.time;

            // 고민하는 시간 랜덤으로 부여
            SetDecisionDuration();

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
            if (CheckTargetPositionOnNavMesh(randomPosition, maxMoveDistance, NavMesh.AllAreas))
            {
                // 타이머 리셋
                ResetDecisionStartTime();
                return NodeState.SUCCESS;
            }

            // 목표가 설정되지 못하면 다시 고민하고 목표 찾기
            ResetDecisionStartTime();
            return NodeState.RUNNING;
        }
    }

    private NodeState MoveToTarget()
    {
        // 목표가 설정되지 않으면 return
        if (!agent.enabled || targetDestination == Vector3.zero || !isMovingToTarget) return NodeState.SUCCESS;


        // 이동 시작
        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            ResetSetting();
            return NodeState.SUCCESS;
        }
        else
        {
            return NodeState.RUNNING;
        }
    }

    #endregion

    #region Sequence : Run Away

    private NodeState CheckDamage()
    {
        // 플레이어로부터 데미지를 받았는지 체크
        NodeState state = (isHit) ? NodeState.SUCCESS : NodeState.FAILURE;

        return state;
    }

    private NodeState Hit()
    {
        // 피격 이펙트 표시
        Debug.Log("데미지 받음");
        return NodeState.SUCCESS;
    }

    private NodeState SetRunTargetPosition()
    {
        if(hitPosition == Vector3.zero) return NodeState.FAILURE;

        // 도망갈 포지션 세팅

        // hit 된 포지션 반대방향으로 설정
        Vector3 backDirection = transform.position - hitPosition;
        backDirection.y = 0;
        Vector3 backDirectionNormalized = backDirection.normalized;

        // 도망거리
        float distance = Random.Range(minMoveDistance, maxMoveDistance);

        Vector3 runDirection = transform.position + backDirectionNormalized * distance;

        // 목표 설정 : 도망가는것 경로를 찾을때까지 반복
        do
        {
            bool isFind = CheckTargetPositionOnNavMesh(runDirection, maxMoveDistance, NavMesh.AllAreas);
        }
        while(CheckTargetPositionOnNavMesh(runDirection, maxMoveDistance, NavMesh.AllAreas)==false);


        return NodeState.SUCCESS;
    }

    #endregion

    private bool CheckTargetPositionOnNavMesh(Vector3 sourcePosition, float maxDistance, int areaMask)
    {
        if(NavMesh.SamplePosition(sourcePosition, out NavMeshHit hit, maxDistance, areaMask))
        {
            targetDestination = hit.position;
            Debug.DrawRay(targetDestination, Vector3.up * 5f, Color.green, 1f);

            agent.SetDestination(targetDestination);
            agent.isStopped = false;

            isMovingToTarget = true; // 플래그 설정

            return true;
        }

        return false;
    }

    protected override void Update()
    {
        base.Update();

        MoveAnimation();
    }

    private void MoveAnimation()
    {
        // Axis 계산
        Vector3 velocity = agent.velocity;
        Vector3 localVelocity = transform.InverseTransformDirection(velocity);
        Vector2 desiredAxis = new Vector2(localVelocity.x, localVelocity.z);

        // run 상태 계산
        float desiredState = (velocity.sqrMagnitude > Mathf.Epsilon) ? 1f : 0f;

        // 애니메이션 적용
        animationHandler.Animate(in desiredAxis, desiredState, Time.deltaTime);
    }

    public void OnHit(Vector3 hitPosition)
    {
        this.hitPosition = hitPosition;
        isHit = true;
    }

    private void ResetSetting()
    {
        agent.isStopped = true;
        targetDestination = Vector3.zero;
        isMovingToTarget = false;
        isHit = false;
    }
}
