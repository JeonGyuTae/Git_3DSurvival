using UnityEngine;

public class UICondition : MonoBehaviour
{
    public Condition health;
    public Condition hunger;
    public Condition thirsty;
    public Condition stamina;

    private void Start()
    {
        PlayerManager.Instance.Player.condition.uiCondition = this;
    }
}
