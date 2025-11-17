using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class Condition : MonoBehaviour
{
    public float curValue;
    public float startValue;
    public float maxValue;
    public float passiveValue;
    public Image uiBar;
    public TextMeshProUGUI figureNumber;

    void Start()
    {
        curValue = startValue;
    }

    void Update()
    {
        uiBar.fillAmount = GetPercentage();
        figureNumber.text = ((int)curValue).ToString();
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
