using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    public PlayerController controller {get; private set;}
    public PlayerCondition condition {get; private set;}

    //private ItemBlahBlah blahblah;
    //private Action blah;

    [SerializeField] private Transform dropPosition;

    private void Awake()
    {
        PlayerManager.Instance.Player = this;
        controller = GetComponent<PlayerController>();
        condition = GetComponent<PlayerCondition>();
    }
}
