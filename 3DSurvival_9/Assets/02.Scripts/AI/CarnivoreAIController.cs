using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.LightAnchor;

public class CarnivoreAIController : AIController
{
    [SerializeField] private float filedOfView = 120f;
    [SerializeField] private float detectDistance = 10f;
    [SerializeField] private float attackDistance = 2.0f;
    [SerializeField] private float attackCoolTime = 3.0f;

    [SerializeField] private int threadHoldRunHp = 30;

    private float currentAttackCoolTime = 0.0f;
    private bool canAttack = false;

    private float playerDistance;

    private bool isMovingToTarget = false; // 플래그 설정
    private AnimalAnimationHandler animationHandler;

    private Vector3 hitPosition;

    [SerializeField] private bool isHit = false;
    public bool IsHit { get { return isHit; } set { isHit = value; } }

    private bool isAttackMode = false;
    private bool isAttack = false;
    private bool isRunMode = false;

    private AnimalConditionHandler conditionHandler;
    private SkinnedMeshRenderer skinnedMeshRenderer;
    private Rigidbody _rigidbody;

    private Coroutine damageEffectCoroutine;

    private Player player;

    protected override void Awake()
    {
        base.Awake();
        animationHandler = GetComponent<AnimalAnimationHandler>();
        skinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        _rigidbody = GetComponent<Rigidbody>();
        conditionHandler = GetComponent<AnimalConditionHandler>();
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

        // Selector Root 생성
        Selector root = new Selector("Root", findMode, patrolMode);

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
        agent.isStopped = false;
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
            return NodeState.RUNNING;
        }
        else
        {
            Vector3 randomDirection = Random.onUnitSphere * Random.Range(minMoveDistance, maxMoveDistance);
            Vector3 randomPosition = transform.position + randomDirection;

            // 목표 설정
            if (CheckTargetPositionOnNavMesh(randomPosition, maxMoveDistance, NavMesh.AllAreas))
            {
                // 속도 세팅
                SetSpeed(runSpeed);

                SetStoppingDistance(0.0f);

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
        // RUN 모드면 SUCCESS
        if (isRunMode) return NodeState.SUCCESS;

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

    private bool CheckTargetPositionOnNavMesh(Vector3 sourcePosition, float maxDistance, int areaMask)
    {
        if (NavMesh.SamplePosition(sourcePosition, out NavMeshHit hit, maxDistance, areaMask))
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
        animationHandler.Animate(in desiredAxis, desiredState, Time.deltaTime);
    }

    public void OnHit(Vector3 hitPosition)
    {
        this.hitPosition = hitPosition;
        isHit = true;

        conditionHandler.TakeDamage(30);
        if (conditionHandler.Health > threadHoldRunHp) isRunMode = true;
    }

    public void OnTakeDamage(int damage)
    {
        isHit = true;
        conditionHandler.TakeDamage(damage);

        if(conditionHandler.Health > threadHoldRunHp) isRunMode = true;
    }

    private void ResetSetting()
    {
        agent.isStopped = true;
        targetDestination = Vector3.zero;
        isMovingToTarget = false;
        isHit = false;
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
        Material mat = skinnedMeshRenderer.material;
        mat.color = Color.red;

        skinnedMeshRenderer.material = mat;

        yield return new WaitForSeconds(0.5f);

        mat.color = Color.white;
        skinnedMeshRenderer.material = mat;
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
}
