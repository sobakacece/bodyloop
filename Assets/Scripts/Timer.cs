using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    private Text timerText;
    private float startTime;

    private float elapsed;

    // Start is called before the first frame update
    void Start()
    {
        startTime = Time.time;
        timerText = GetComponent<Text>();
        GameFlow.Instance.winEvent += () => GameFlow.Instance.currentTime = elapsed;
    }

    // Update is called once per frame
    void Update()
    {
    elapsed = Time.time - startTime;
    timerText.text = $"{(int)(elapsed / 60):00}:{(int)(elapsed % 60):00}";
        
    }
}
