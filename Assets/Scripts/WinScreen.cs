using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WinScreen : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private Button buttonRestart;
    [SerializeField]
    private Button buttonQuit;
    [SerializeField]
    private GameObject mainPanel;
    [SerializeField]
    private Text currentTime;

    void Start()
    {
        buttonRestart.onClick.AddListener(GoBack);
        buttonQuit.onClick.AddListener(GameFlow.Instance.Return);
        SetCurrenTime();
        mainPanel.transform.position = new Vector3(2560 + 1280, 720, 0);
        PlayAnimation(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetCurrenTime()
    {
        double time = GameFlow.Instance.currentTime;
        int minutes = (int)(time / 60);
        int seconds = (int)(time % 60);
        int microseconds = (int)((time - Math.Truncate(time)) * 1_000) % 1_000;

        currentTime.text = $"{minutes:00}:{seconds:00}:{microseconds:000}";
    }
    public void GoBack()
    {
        PlayAnimation(true);
        Destroy(this);
        GameFlow.Instance.GameRestart();
    }

    void PlayAnimation(bool reverse)
    {
        if (reverse)
        {
            mainPanel.transform.LeanMoveLocal(new Vector2(2560 + 1280, 0), 1).setEaseOutQuart().setIgnoreTimeScale(true);
        }
        else
        {
            mainPanel.transform.LeanMoveLocal(new Vector2(0, 0), 1).setEaseOutQuart().setIgnoreTimeScale(true);

        }
    }
}
