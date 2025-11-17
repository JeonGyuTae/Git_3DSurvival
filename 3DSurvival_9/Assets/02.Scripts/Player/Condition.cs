using UnityEngine.UI;
using UnityEngine;

public class Condition : MonoBehaviour
{
    [SerializeField] private float curValue;
    [SerializeField] private float startValue;
    [SerializeField] private float maxValue;
    [SerializeField] private float passiveValue;
    // public Image uiBar;

    void Start()
    {
        curValue = startValue;
    }

    void Update()
    {
        // uiBar.fillAmount = GetPercentage();
    }

    float GetPercentage()
    {
        return curValue / maxValue;
    }

    public void Add(float value)
    {
        curValue = Mathf.Min(curValue + value, maxValue);
    }

    public void Sub(float value)
    {
        curValue = Mathf.Max(curValue  - value, 0);
    }
}
