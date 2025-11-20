using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageIndicator : MonoBehaviour
{
    [SerializeField] private Image effectLow;
    [SerializeField] private Image effectHigh;
    [SerializeField] private Image effectHeal;
    [SerializeField] private float flashSpeed;

    private Coroutine coroutine;

    private void Start()
    {
        PlayerManager.Instance.Player.condition.OnTakeDamageToHalf += FlashLow;
        PlayerManager.Instance.Player.condition.OnTakeDamageToZero += FlashHigh;
        PlayerManager.Instance.Player.condition.OnHeal += FlashHeal;

        effectLow.enabled = false;
        effectHigh.enabled = false;
        effectHeal.enabled = false;
    }

    public void FlashLow()
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }

        effectLow.enabled = true;
        effectLow.color = new Color(1f, 1f, 1f);
        coroutine = StartCoroutine(FadeAway(effectLow, 0.3f));
    }

    public void FlashHigh()
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }

        effectHigh.enabled = true;
        effectHigh.color = new Color(1f, 1f, 1f);
        coroutine = StartCoroutine(FadeAway(effectHigh, 0.5f));
    }

    public void FlashHeal()
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }

        effectHeal.enabled = true;
        effectHeal.color = new Color(1f, 1f, 1f);
        coroutine = StartCoroutine(FadeAway(effectHeal, 0.5f));
    }

    private IEnumerator FadeAway(Image effect, float startAlpha)
    {
        float alpha = startAlpha;

        while (alpha > 0)
        {
            alpha -= (startAlpha / flashSpeed) * Time.deltaTime;
            effect.color = new Color(255f, 255f, 255f, alpha);
            yield return null;
        }

        effect.enabled = false;
    }
}
