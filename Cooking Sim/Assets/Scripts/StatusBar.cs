using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TimerState { Stopwatch, Countdown };

public class StatusBar : MonoBehaviour
{
    public delegate void OnTimerComplete();
    public OnTimerComplete onTimerCompleteCallback;

    public float percentFilled;

    [SerializeField] Transform barFill;
    [SerializeField] Transform barEmpty;
    [SerializeField] Color fillColor;
    [SerializeField] Color emptyColor;

    private float maxFillWidthScale;
    private float fillHeightScale;
    private float totalTime;
    private float timer;
    private bool timerActive = false;
    private TimerState timerState;
    private float speedMultiplier = 1f;
    
    void Start()
    {
        // Determine what scale settings are for bar in full state 
        maxFillWidthScale = barFill.transform.localScale.x;
        fillHeightScale = barFill.transform.localScale.y;
    }

    private void Update()
    {

        if (!timerActive) return;

        // Count up or down depending we're using stopwatch or countdown mode
        switch (timerState)
        {
            case TimerState.Countdown:
                timer -= Time.deltaTime * speedMultiplier;
                break;
            case TimerState.Stopwatch:
                timer += Time.deltaTime;
                break;
        }

        UpdateBar();

        // If countdown or stopwatch is complete invoke callback and disable timer
        if (timer <= 0 || timer > totalTime)
        {
            onTimerCompleteCallback?.Invoke();
            timerActive = false;
        }
    }

    private void UpdateBar()
    {
        percentFilled = timer / totalTime;
        float barFillWidthScale = percentFilled * maxFillWidthScale;
        barFill.transform.localScale = new Vector3(barFillWidthScale, fillHeightScale, 1f);
    }

    public void StartCountdownTimer(float duration)
    {
        timerState = TimerState.Countdown;
        speedMultiplier = 1f;
        totalTime = duration;
        timer = totalTime;
        timerActive = true;
    }

    public void StartStopWatchTimer(float duration)
    {
        timerState = TimerState.Stopwatch;
        totalTime = duration;
        timer = 0f;
        timerActive = true;
    }

    // Used by Speed powerup to increase amount of time counter counts down
    public void SetSpeedMultiplier(float multiplier)
    {
        speedMultiplier = multiplier;
    }
}
