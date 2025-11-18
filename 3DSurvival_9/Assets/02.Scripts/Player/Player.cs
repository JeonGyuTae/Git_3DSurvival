using UnityEngine;

public class Player : MonoBehaviour
{
    public PlayerController controller {get; private set;}
    public PlayerCondition condition {get; private set;}

    public ItemData itemData;
    public System.Action addItem;

    public Transform dropPosition;

    private void Awake()
    {
        PlayerManager.Instance.Player = this;
        controller = GetComponent<PlayerController>();
        condition = GetComponent<PlayerCondition>();
    }
}
