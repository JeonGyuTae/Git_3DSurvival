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

    protected virtual void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    protected virtual void Start()
    {

    }

    protected abstract void SetBehavior();

    protected virtual void Update()
    {
        // BehaviorTree 실행
        tree.RunTree();
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
}
