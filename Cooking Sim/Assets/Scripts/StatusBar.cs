using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusBar : MonoBehaviour
{
    public delegate void OnTimerComplete();
    public OnTimerComplete onTimerCompleteCallback;

    [SerializeField] Transform barFill;
    [SerializeField] Transform barEmpty;
    [SerializeField] Color fillColor;
    [SerializeField] Color emptyColor;

    private float maxFillWidthScale;
    private float fillHeightScale;

    private float totalTime;
    private float timer;
    private bool timerActive = false;

    public float percentFilled
    {
        get
        {
            return barFill.transform.localScale.x / maxFillWidthScale;
        }
    }

    void Start()
    {
        maxFillWidthScale = barFill.transform.localScale.x;
        fillHeightScale = barFill.transform.localScale.y;
    }

    private void Update()
    {
        if(timerActive)
        {
            timer -= Time.deltaTime;
            SetFillPercent(timer / totalTime);

            if (timer <= 0)
            {
                onTimerCompleteCallback?.Invoke();
                timerActive = false;
            }
        }
    }

    public void SetFillPercent(float fillPercent)
    {
        float barFillWidthScale = fillPercent * maxFillWidthScale;
        barFill.transform.localScale = new Vector3(barFillWidthScale, fillHeightScale, 1f);
    }

    public void StartCountdownTimer(float duration)
    {
        totalTime = duration;
        timer = totalTime;
        timerActive = true;
    }
}
