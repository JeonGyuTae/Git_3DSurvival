using Controller;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;


/// <summary>
/// 모든 AI가 상속받는 Controller 클래스
/// BehaviorTree 구조로 움직임을 구현하며
/// AI Navigation을 연동하여 활동 지역을 지정함
/// </summary>
public abstract class AIController : MonoBehaviour
{
    [Header("AI")]
    [SerializeField] protected NavMeshAgent agent;
    [SerializeField] protected float moveSpeed = 5.0f;
    [SerializeField] protected float runSpeed = 10.0f;
    [SerializeField] protected float maxMoveDistance = -10.0f;
    [SerializeField] protected float minMoveDistance = 10.0f;
    [SerializeField] protected float maxDecisionDuration = 2f;
    [SerializeField] protected float minDecisionDuration = 5f;
    protected float decisionDuration = 3f;

    protected BehaviorTree tree;
    protected float decisionStartTime = -1f;
    protected Vector3 targetDestination;
    protected Vector3 currentDestination;

    [Header("DeadCoolTime")]
    [SerializeField] private float deadCoolTime = 2.0f;
    private float currentDeadCoolTime = 0.0f;
    private bool canDestory = false;

    private bool isMovingToTarget = false; // 플래그 설정
    private int cntFindPatrolPath = 0;      // 순회 결정 카운트
    private int threadHoldFindPatrolPathCount = 30;  // 최대 순회 결정 카운트

    protected Vector3 hitPosition;

    [SerializeField] protected bool isHit = false;
    public bool IsHit { get { return isHit; } set { isHit = value; } }

    protected Animal animal;
    protected Coroutine deadCoroutine;
    protected WaitForSeconds waitForSeconds;

    protected virtual void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animal = GetComponent<Animal>();
    }

    public void Init()
    {
        // 활성화 시 세팅
        currentDeadCoolTime = 0.0f;
        canDestory = false;
    }

    protected virtual void Start()
    {
        waitForSeconds = new WaitForSeconds(2f);
    }

    protected abstract void SetBehavior();

    protected virtual void Update()
    {
        // BehaviorTree 실행
        tree.RunTree();

        UpdateDeadCoolTime();
    }

    #region Sequence : Dead

    protected NodeState IsDead()
    {
        NodeState state = (animal.ConditionHandler.Health <= 0) ? NodeState.SUCCESS : NodeState.FAILURE;
        return state;
    }

    protected NodeState Dead()
    {
        SetAgentStop(true);
        this.gameObject.transform.localRotation = Quaternion.Euler(0f, 0f, 90f);

        if (canDestory)
        {
            // 터지는 이펙트 적용
            animal.FX_Dead.Play();

            canDestory = false;
            SetAgentStop(false);
            animal.SkinnedMeshRenderer.enabled = false;

            Invoke("DisableObject", deadCoolTime);

            // 리스폰 할 수 있도록 설정
            AnimalSpawnManager.Instance.Respawn(animal);

            return NodeState.SUCCESS;
        }

        return NodeState.RUNNING;
    }

    private void UpdateDeadCoolTime()
    {
        if (animal.ConditionHandler.Health > 0) return;

        // 죽었을 경우만 상태 쿨타임 업데이트
        currentDeadCoolTime += Time.deltaTime;
        if (currentDeadCoolTime >= deadCoolTime)
        {
            currentDeadCoolTime = 0.0f;
            canDestory = true;
        }
    }

    private void DisableObject()
    {
        // 오브젝트 비활성화
        string key = animal.Data.animalName;
        GameObject obj = this.gameObject;
        AnimalSpawnManager.Instance.Release(key, obj);
    }

    #endregion

    #region Sequence : Patrol

    protected NodeState SetPatrolTargetPosition()
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

            // 선택한 목표가 서식지 영역 내에 있는지 확인
            if (!IsPositionInHabitatArea(randomPosition))
            {
                cntFindPatrolPath++;

                CheckFindPatrolPath();

                ResetDecisionStartTime();
                return NodeState.RUNNING;
            }

            // 목표 설정 : 선택한 목표가 서식지 영역 내에 있는지 확인
            if (CheckTargetPositionOnNavMesh(randomPosition, maxMoveDistance, NavMesh.AllAreas))
            {
                // 속도 세팅
                SetSpeed(runSpeed);

                // 타이머 리셋
                ResetDecisionStartTime();

                // 순회 카운트 리셋
                cntFindPatrolPath = 0;

                return NodeState.SUCCESS;
            }

            cntFindPatrolPath++;

            CheckFindPatrolPath();

            // 타이머 리셋
            ResetDecisionStartTime();
            return NodeState.RUNNING;
        }
    }

    private void CheckFindPatrolPath()
    {
        // 만약 장시간 순회위치를 못찾은 다면 위치 재설정
        if (cntFindPatrolPath > threadHoldFindPatrolPathCount)
        {
            bool successWarp;
            Vector3 resetPosition;
            do
            {
                resetPosition = GetRandomPositionInArea();
                successWarp = agent.Warp(resetPosition);
            }
            while (successWarp == false);

            cntFindPatrolPath = 0;
            Debug.Log($"{animal.Data.animalName} 장기간 순회 위치 못찾아서 위치 재설정");
        }
    }

    protected bool IsPositionInHabitatArea(Vector3 position)
    {
        // pivot 위치와 habitat 가져오기
        Vector3 pivotPos = animal.Data.pivotArea;
        Vector3 habitat = animal.Data.habitat;

        // x 경계 확인
        float minX = pivotPos.x - habitat.x / 2;
        float maxX = pivotPos.x + habitat.x / 2;
        bool inX = (position.x >= minX) && (position.x <= maxX);

        // z 경계 확인
        float minZ = pivotPos.z - habitat.z / 2;
        float maxZ = pivotPos.z + habitat.z / 2;
        bool inZ = (position.z >= minX) && (position.z <= maxX);

        // 해당 Position이 Habitat 영역 내에 있는지 반환
        return inX && inZ;
    }

    private Vector3 GetRandomPositionInArea()
    {
        Vector3 pivotPos = animal.Data.pivotArea;

        // X, Z 축은 Area 영역 내 지정
        float randomPosX = Random.Range(pivotPos.x - animal.Data.habitat.x / 2, pivotPos.x + animal.Data.habitat.x / 2);
        float randomPosZ = Random.Range(pivotPos.z - animal.Data.habitat.z / 2, pivotPos.z + animal.Data.habitat.z / 2);

        // Y 축은 일정 offset 범위 위에서 지정 (땅 위에서 생성될 수 있도록)
        float posY = pivotPos.y + Random.Range(0, 4f);

        // 위치 반환
        Vector3 spawnPos = new Vector3(randomPosX, posY, randomPosZ);
        return spawnPos;
    }

    protected NodeState MoveToTarget()
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

    protected bool CheckTargetPositionOnNavMesh(Vector3 sourcePosition, float maxDistance, int areaMask)
    {
        if (NavMesh.SamplePosition(sourcePosition, out NavMeshHit hit, maxDistance, areaMask))
        {
            targetDestination = hit.position;
            Debug.DrawRay(targetDestination, Vector3.up * 5f, Color.green, 1f);

            agent.SetDestination(targetDestination);
            SetAgentStop(false);

            isMovingToTarget = true; // 플래그 설정

            return true;
        }

        return false;
    }

    protected void ResetSetting()
    {
        SetAgentStop(true);
        targetDestination = Vector3.zero;
        isMovingToTarget = false;
        isHit = false;
    }

    public virtual void OnHit(Vector3 hitPosition)
    {
        // 피격 판정
    }

    protected void ResetDecisionStartTime()
    {
        // 타이머 초기화
        decisionStartTime = -1f;
    }

    protected void SetDecisionDuration()
    {
        // 고민하는 시간은 랜덤으로 설정
        decisionDuration = Random.Range(minDecisionDuration, maxDecisionDuration);
    }

    protected void SetSpeed(float speed)
    {
        agent.speed = speed;
    }

    protected void SetStoppingDistance(float distance)
    {
        agent.stoppingDistance = distance;
    }

    public void SetAgentStop(bool isStop)
    {
        agent.isStopped = isStop;
    }

    #region 프로퍼티

    public NavMeshAgent Agent { get { return agent; } }

    #endregion
}
