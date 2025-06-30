using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Diagnostics;
using System;

public class Timer : MonoBehaviour
{
    private Text timerText;
    private Stopwatch stopwatch;

    public double elapsed; // In seconds with sub-millisecond precision

    void Start()
    {
        timerText = GetComponent<Text>();
        stopwatch = new Stopwatch();
        stopwatch.Start();

        GameFlow.Instance.winEvent += () =>
        {
            stopwatch.Stop();
            elapsed = stopwatch.Elapsed.TotalSeconds;
            GameFlow.Instance.currentTime = elapsed;
        };
    }

    void Update()
    {
        double time = stopwatch.Elapsed.TotalSeconds;
        int minutes = (int)(time / 60);
        int seconds = (int)(time % 60);
        int microseconds = (int)((time - Math.Truncate(time)) * 1_000) % 1_000;

        timerText.text = $"{minutes:00}:{seconds:00}:{microseconds:000}";
    }
}
