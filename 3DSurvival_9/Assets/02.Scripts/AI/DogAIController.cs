using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.AI;

public class DogAIController : AIController
{
    [Header("Dog Stat")]
    [SerializeField] private float attackDistance = 2.0f;
    [SerializeField] private float attackCoolTime = 3.0f;

    // 공격 상태 변수
    private float currentAttackCoolTime = 0.0f;
    private bool canAttack = false;

    // 플레이어와 거리를 체크하는 함수
    private float playerDistance;

    // 공격 대상
    private Transform target = null;

    // 상태 변수
    private bool isAttackMode = false;
    private bool isAttack = false;

    // 코루틴 변수
    private Coroutine damageEffectCoroutine;

    // 플레이어 참조
    private Player player;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        // BehaviorTree 설정
        SetBehavior();

        ResetDecisionStartTime();

        currentDestination = transform.position;

        player = PlayerManager.Instance.Player;
    }

    protected override void SetBehavior()
    {
        // Dead
        ConditionNode isDead = new ConditionNode("IsDead", IsDead);
        Action dead = new Action("Dead", Dead);

        // 플레이어 따라다니기
        Action moveToTarget = new Action("Move Next Position", MoveToTarget);
        Action setTargetToPlayer = new Action("Set Target To Player", SetTargetToPlayer);
        ConditionNode checkDamage = new ConditionNode("Check Damage From Player", CheckDamage);
        Action hit = new Action("Hit", Hit);
        Action setRunTargetPosition = new Action("Set Run Position", SetRunTargetPosition);

        // Sequence Patrol 생성
        Sequence followPlayer = new Sequence("Follow Player", setTargetToPlayer, moveToTarget);

        // Sequence RunAway 생성
        Sequence runAway = new Sequence("RunAway", checkDamage, hit, setRunTargetPosition, moveToTarget);

        // 따라가기 모드
        Selector followMode = new Selector("Follow Mode", runAway, followPlayer);

        // 공격 모드
        ConditionNode isTargetExist = new ConditionNode("IsTargetExist?", IsTargetExist);
        Action setTarget = new Action("SetTarget", SetTarget);
        Action attack = new Action("Attack Player", Attack);

        // Sequence FindMode 생성
        Sequence attackSequence = new Sequence("Attack Sequence", setTarget, moveToTarget, attack);
        Selector attackMode = new Selector("Attack Mode", runAway, attackSequence);
        Sequence findMode = new Sequence("Find Mode", isTargetExist, attackMode);

        // Sequence Dead 생성
        Sequence checkDead = new Sequence("Check Dead", isDead, dead);

        // Selector Root 생성
        Selector root = new Selector("Root", checkDead, findMode, followMode);

        tree = new BehaviorTree(root);
    }

    #region 공격 모드

    private NodeState IsTargetExist()
    {
        return (target != null) ? NodeState.SUCCESS : NodeState.FAILURE;
    }

    private NodeState SetTarget()
    {
        // 속도 세팅
        SetSpeed(runSpeed);

        // 타켓 위치로 목표 설정
        SetAgentStop(false);
        NavMeshPath path = new NavMeshPath();
        if (agent.CalculatePath(target.transform.position, path))
        {
            agent.SetDestination(target.transform.position);
            targetDestination = target.transform.position;
            SetStoppingDistance(2.0f);
            return NodeState.SUCCESS;
        }

        return NodeState.RUNNING;
    }

    private NodeState Attack()
    {
        // AttackCoolTime인지 체크

        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            if (canAttack)
            {
                player.condition.TakeDamage((int)animal.Data.atk);
                isAttack = true;
                canAttack = false;
                currentAttackCoolTime = 0;
                return NodeState.SUCCESS;
            }
        }

        return NodeState.FAILURE;
    }

    #endregion

    #region Sequence : Run Away

    private NodeState CheckDamage()
    {
        // 데미지를 받았는지 체크
        if (isHit) return NodeState.SUCCESS;

        return NodeState.FAILURE;
    }

    private NodeState Hit()
    {
        // 피격 이펙트 표시
        if (isHit)
        {
            StartDamageEffectCoroutine();
        }

        return NodeState.SUCCESS;
    }

    private NodeState SetRunTargetPosition()
    {
        if (target.transform.position == Vector3.zero) return NodeState.FAILURE;

        // 도망갈 포지션 세팅

        // 속도 세팅
        SetSpeed(runSpeed);

        // hit 된 포지션 반대방향으로 설정
        Vector3 backDirection = transform.position - target.transform.position;
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
        while (CheckTargetPositionOnNavMesh(runDirection, maxMoveDistance, NavMesh.AllAreas) == false);

        SetStoppingDistance(0.0f);

        return NodeState.SUCCESS;
    }

    #endregion

    #region 플레이어 따라다니기 Sequence

    private NodeState SetTargetToPlayer()
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
            return NodeState.RUNNING;
        }
        else
        {
            // 속도 세팅
            SetSpeed(runSpeed);

            // 타켓 위치로 목표 설정
            SetAgentStop(false);
            NavMeshPath path = new NavMeshPath();
            if (agent.CalculatePath(player.transform.position, path))
            {
                agent.SetDestination(player.transform.position);
                SetAgentStop(false);
                isMovingToTarget = true; // 플래그 설정
                targetDestination = player.transform.position;
                float randomDistance = Random.Range(3.0f, 8.0f);
                SetStoppingDistance(randomDistance);

                // 타이머 리셋
                ResetDecisionStartTime();

                return NodeState.SUCCESS;
            }

            return NodeState.RUNNING;
        }
    }

    #endregion

    protected override void Update()
    {
        base.Update();

        AnimationHandle();
        UpdateAttackCoolTime();
    }

    private void AnimationHandle()
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

    public void SetTarget(Transform target)
    {
        this.target = target;
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

    private void UpdateAttackCoolTime()
    {
        if (currentAttackCoolTime >= attackCoolTime)
        {
            return;
        }

        currentAttackCoolTime += Time.deltaTime;
        if (currentAttackCoolTime >= attackCoolTime)
        {
            currentAttackCoolTime = attackCoolTime;
            canAttack = true;
        }
    }
}
