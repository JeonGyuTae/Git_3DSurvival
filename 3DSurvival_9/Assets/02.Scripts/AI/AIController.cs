using Controller;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


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
