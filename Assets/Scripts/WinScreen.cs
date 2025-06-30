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
    private Button buttonLeaderboard;
    [SerializeField]
    private GameObject mainPanel;
    [SerializeField]
    private Text currentTime;
    private Leaderboard leaderboard;

    void Start()
    {
        buttonRestart.onClick.AddListener(GoBack);
        buttonQuit.onClick.AddListener(GameFlow.Instance.Return);
        buttonLeaderboard.onClick.AddListener(CallLeaderboard);
        SetCurrenTime();
        mainPanel.transform.position = new Vector3(2560 + 1280, 720, 0);
        PlayAnimation(false);
        GameObject prefab = Resources.Load<GameObject>("Prefab/Leaderboard");
        leaderboard = Instantiate(prefab, gameObject.transform).GetComponent<Leaderboard>();
        leaderboard.AddRow(currentTime.text);
    }
    private void CallLeaderboard()
    {
        leaderboard.gameObject.SetActive(false);
        leaderboard.Appear();
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
