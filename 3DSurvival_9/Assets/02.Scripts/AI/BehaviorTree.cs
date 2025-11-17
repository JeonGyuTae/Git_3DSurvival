using System;
using System.Collections.Generic;
using UnityEngine;

public enum NodeState
{
    SUCCESS,
    FAILURE,
    RUNNING,
    UNDEF
}

/// <summary>
/// BehaviorTree 클래스
/// Root가 되는 Node를 가지고 있다.
/// 기본적으로 SUCCESS시 다음 노드로 넘어감
/// </summary>
public class BehaviorTree
{
    private Node root;

    public BehaviorTree(Node rootNode)
    {
        root = rootNode;
        root.TagCondition();
    }

    public void RunTree()
    {
        NodeState result = root.Run();

        if(result == NodeState.SUCCESS)
        {
            root.Reset();
        }
    }
}

/// <summary>
/// Node 클래스
/// 이름과 자식 노드 리스트를 가지고 있음
/// </summary>
public abstract class Node
{
    public NodeState state = NodeState.UNDEF;
    public string name;
    public bool hasCondition;
    public List<Node> children = new List<Node>();

    public virtual void AddChild(Node node)
    {
        children.Add(node);
    }

    public virtual void TagCondition()
    {
        foreach (var child in children)
        {
            child.TagCondition();
            if (child.hasCondition)
                hasCondition = true;
        }
    }

    public void Reset()
    {
        state = NodeState.UNDEF;
        foreach (var child in children)
        {
            child.Reset();
        }
    }

    public abstract NodeState Run();
}

/// <summary>
/// Sequence 노드
/// 자식 노드 모두가 성공할 때까지 순차적 실행
/// </summary>
public class Sequence : Node
{
    public Sequence(string name, params Node[] childs)
    {
        this.name = name;
        children.AddRange(childs);
    }

    public override NodeState Run()
    {
        foreach(var child in children)
        {
            if(child.state == NodeState.UNDEF || child.state == NodeState.RUNNING || child.hasCondition)
            {
                state = child.Run();

                // 하나라도 실행 중이거나 실패하면 반환
                if(state == NodeState.RUNNING || state == NodeState.FAILURE)
                {
                    return state;
                }
            }
        }

        // 모두 성공 시 SUCCESS 반환
        state = NodeState.SUCCESS;
        return state;
    }
}

/// <summary>
/// Selector 노드
/// 자식 노드 중 하나라도 성공할때 까지 순차적 실행
/// </summary>
public class Selector : Node
{
    public Selector(string name, params Node[] childs)
    {
        this.name = name;
        children.AddRange(childs);
    }

    public override NodeState Run()
    {
        foreach(var child in children)
        {
            if (child.state == NodeState.UNDEF || child.state == NodeState.RUNNING || child.hasCondition)
            {
                state = child.Run();

                // 자식 노드 중 하나라도 RUNNING이나 SUCCESS 반환 하면, 그 상태 반환
                if (state == NodeState.RUNNING || state == NodeState.SUCCESS)
                {
                    return state;
                }
            }
        }

        // 모든 자식이 실패하면 실패 반환
        state = NodeState.FAILURE;
        return state;
    }
}


/// <summary>
/// Action 노드
/// 실제 게임 로직을 담당하는 노드
/// </summary>
public class Action : Node
{
    private Func<NodeState> actionFunc;     // 실제 로직을 담당할 메서드

    public Action(string name, Func<NodeState> func)
    {
        this.name = name;
        this.actionFunc = func;
    }

    public override void TagCondition()
    {
        // Action 노드는 condition을 가지고 있지 않음
        hasCondition = false;
    }

    public override void AddChild(Node node)
    {
        Debug.Log("Action 노드는 자식을 가지지 못함");
    }

    public override NodeState Run()
    {
        // 로직 실행
        state = actionFunc.Invoke();
        return state;
    }
}

/// <summary>
/// Condition 노드
/// 특정 조건을 만족하는지 확인하는 노드
/// </summary>
public class Condition : Node
{
    private Func<NodeState> condictionFunc;     // 조건을 판별할 메서드

    public Condition(string name, Func<NodeState> func)
    {
        this.name = name;
        condictionFunc = func;
    }

    public override void TagCondition()
    {
        // Condition 노드는 condition을 가지고 있음
        hasCondition = true;
    }

    public override void AddChild(Node node)
    {
        Debug.Log("Condition 노드는 자식을 가지지 못함");
    }

    public override NodeState Run()
    {
        // 조건 실행
        state = condictionFunc.Invoke();

        if(state == NodeState.RUNNING)
        {
            Debug.Log("Condition 메서드는 RUNNING을 반환 못함");
        }

        // 조건 상태 반환
        return state;
    }
}
