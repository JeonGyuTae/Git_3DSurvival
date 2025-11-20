using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.LightAnchor;

public class CarnivoreAIController : AIController
{
    [Header("Carnivor Stat")]
    [SerializeField] private float filedOfView = 120f;
    [SerializeField] private float detectDistance = 10f;
    [SerializeField] private float attackDistance = 2.0f;
    [SerializeField] private float attackCoolTime = 3.0f;

    [SerializeField] private int threadHoldRunHp = 30;

    // 공격 상태 변수
    private float currentAttackCoolTime = 0.0f;
    private bool canAttack = false;

    // 플레이어와 거리를 체크하는 함수
    private float playerDistance;

    // 상태 변수
    private bool isAttackMode = false;
    private bool isAttack = false;
    private bool isRunMode = false;

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

        // 순회
        Action moveToTarget = new Action("Move Next Position", MoveToTarget);
        Action setPatrolTargetPosition = new Action("Decision Next Position", SetPatrolTargetPosition);
        ConditionNode checkDamage = new ConditionNode("Check Damage From Player", CheckDamage);
        Action hit = new Action("Hit", Hit);
        Action setRunTargetPosition = new Action("Set Run Position", SetRunTargetPosition);

        // Sequence Patrol 생성
        Sequence patrol = new Sequence("Patrol Around", setPatrolTargetPosition, moveToTarget);

        // Sequence RunAway 생성
        Sequence runAway = new Sequence("RunAway", checkDamage, hit, setRunTargetPosition, moveToTarget);

        // 순회 모드
        Selector patrolMode = new Selector("Patrol Mode", runAway, patrol);

        // 공격 모드
        ConditionNode findPlayer = new ConditionNode("Is Find Player?", FindPlayer);
        Action setTargetToPlayer = new Action("SetTargetToPlayer", SetTargetToPlayer);
        Action attack = new Action("Attack Player", Attack);

        // Sequence FindMode 생성
        Sequence attackSequence = new Sequence("Attack Sequence", setTargetToPlayer, moveToTarget, attack);
        Selector attackMode = new Selector("Attack Mode", runAway, attackSequence);
        Sequence findMode = new Sequence("Find Mode", findPlayer, attackMode);

        // Sequence Dead 생성
        Sequence checkDead = new Sequence("Check Dead", isDead, dead);

        // Selector Root 생성
        Selector root = new Selector("Root", checkDead, findMode, patrolMode);

        tree = new BehaviorTree(root);
    }

    #region 공격 모드

    private NodeState FindPlayer()
    {
        // RUN 모드면 FAIL
        if (isRunMode) return NodeState.FAILURE;

        // 이미 공격모드면 SUCCESS
        if (isAttackMode) return NodeState.SUCCESS;

        // 플레이어 찾기
        playerDistance = Vector3.Distance(transform.position, player.transform.position);

        if(playerDistance < detectDistance && IsPlayerInFieldOfView())
        {
            isAttackMode = true;
            return NodeState.SUCCESS;
        }

        return NodeState.FAILURE;
    }

    private bool IsPlayerInFieldOfView()
    {
        Vector3 directionToPlayer = PlayerManager.Instance.Player.transform.position - transform.position;
        float angle = Vector3.Angle(transform.forward, directionToPlayer);
        return angle < filedOfView * 0.5f;
    }

    private NodeState SetTargetToPlayer()
    {
        // 속도 세팅
        SetSpeed(runSpeed);

        // 플레이어 
        SetAgentStop(false);
        NavMeshPath path = new NavMeshPath();
        if (agent.CalculatePath(player.transform.position, path))
        {
            agent.SetDestination(player.transform.position);
            targetDestination = player.transform.position;
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
            if(canAttack)
            {
                player.condition.TakeDamage((int)animal.Data.atk);
                Debug.Log("공격!");
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
        // 플레이어로부터 데미지를 받았는지 체크
        if (isHit) return NodeState.SUCCESS;

        // RUN 모드면 SUCCESS
        if (isRunMode) return NodeState.SUCCESS;

        return NodeState.FAILURE;
    }

    private NodeState Hit()
    {
        // 피격 이펙트 표시
        if (isHit)
        {
            StartDamageEffectCoroutine();

            // 피격 당할 시 공격모드
            isAttackMode = true;
        }

        return NodeState.SUCCESS;
    }

    private NodeState SetRunTargetPosition()
    {
        if (hitPosition == Vector3.zero) return NodeState.FAILURE;

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
        while (CheckTargetPositionOnNavMesh(runDirection, maxMoveDistance, NavMesh.AllAreas) == false);

        SetStoppingDistance(0.0f);

        return NodeState.SUCCESS;
    }

    #endregion

    protected override void Update()
    {
        base.Update();

        AnimationHandle();
        UpdateAttackCoolTime();
        UpdateRunMode();
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
        if (animal.ConditionHandler.Health < threadHoldRunHp)
        {
            isAttackMode = false;
            isRunMode = true;
        }
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
        if(currentAttackCoolTime >= attackCoolTime)
        {
            return;
        }

        currentAttackCoolTime += Time.deltaTime;
        if(currentAttackCoolTime >= attackCoolTime)
        {
            currentAttackCoolTime = attackCoolTime;
            canAttack = true;
        }
    }

    private void UpdateRunMode()
    {
        if(Vector3.Distance(player.transform.position, transform.position) > 20.0f)
        {
            isRunMode = false;
        }
    }
}
