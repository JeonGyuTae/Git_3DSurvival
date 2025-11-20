using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.XR;
using static UnityEngine.LightAnchor;

/// <summary>
/// 초식동물 행동 AI Behavior
/// 
///                                       <Selector(Root)>
///             
///            <Sequence(RunAway)>                               <Sequence(Patrol)>         
/// <Condition(피격)>       <Action(RunAway)>       <Action(SetPosition)>       <Action(Move)> 
/// 
/// </summary>
public class HerbivoreAIController : AIController
{
    // 피격 코루틴
    private Coroutine damageEffectCoroutine;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();

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

        ConditionNode isDead = new ConditionNode("IsDead", IsDead);
        Action dead = new Action("Dead", Dead);

        // Sequence Patrol 생성
        Sequence patrol = new Sequence("Patrol Around", setPatrolTargetPosition, moveToTarget);

        // Sequence RunAway 생성
        Sequence runAway = new Sequence("RunAway", checkDamage, hit, setRunTargetPosition, moveToTarget);

        // Sequence Dead 생성
        Sequence checkDead = new Sequence("Check Dead", isDead, dead);

        // Selector Root 생성
        Selector root = new Selector("Chicken Root", checkDead, runAway, patrol);

        tree = new BehaviorTree(root);
    }

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
        StartDamageEffectCoroutine();

        return NodeState.SUCCESS;
    }

    private NodeState SetRunTargetPosition()
    {
        if(hitPosition == Vector3.zero) return NodeState.FAILURE;

        // 도망갈 포지션 세팅

        // 속도 세팅
        SetSpeed(runSpeed);

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
        animal.AnimationHandler.Animate(in desiredAxis, desiredState, Time.deltaTime);
    }

    public override void OnHit(int damage, Vector3 hitPosition)
    {
        this.hitPosition = hitPosition;
        isHit = true;

        animal.ConditionHandler.TakeDamage(damage);
    }

    /// <summary>
    /// 피격 효과
    /// </summary>
    /// <returns></returns>
    private void StartDamageEffectCoroutine()
    {
        if (damageEffectCoroutine != null) StopCoroutine(damageEffectCoroutine);
        damageEffectCoroutine = StartCoroutine(DamageEffect());
    }

    private IEnumerator DamageEffect()
    {
        Material mat = animal.SkinnedMeshRenderer.material;
        mat.color = Color.red;

        animal.SkinnedMeshRenderer.material = mat;

        yield return new WaitForSeconds(0.5f);

        mat.color = Color.white;
        animal.SkinnedMeshRenderer.material = mat;
    }
}
