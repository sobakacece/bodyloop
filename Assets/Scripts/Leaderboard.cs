using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;

public class Leaderboard : MonoBehaviour
{
    private List<LeaderboardStruct> leadearsArray = new List<LeaderboardStruct>();
    private int maxLeaders = 10;
    [SerializeField]
    private VerticalLayoutGroup mainPanel;
    [SerializeField]
    private GameObject leaderBoardRow;
    private string playerName = "Player1";
    [SerializeField]
    private Button button;

    [SerializeField]
    // Start is called before the first frame update
    public void AddRow(float time)
    {
        leadearsArray = GameFlow.Instance.leaderboardSave;
        LeaderboardStruct newRow = new LeaderboardStruct(time, playerName);
        leadearsArray.Add(newRow);
        leadearsArray  = leadearsArray.OrderBy(x => x.MyTime).ToList();
        if (leadearsArray.Count > maxLeaders)
        {
            leadearsArray.RemoveRange(maxLeaders, leadearsArray.Count - maxLeaders);

        }
        for (int i = 0; i < leadearsArray.Count; i++)
        {

            leadearsArray[i].MyPlace = i + 1;

        }
        GameFlow.Instance.leaderboardSave = leadearsArray;
        FillTable();
    }

    void FillTable()
    {
        foreach (Transform child in mainPanel.gameObject.transform)
        {
            Destroy(child.gameObject);
        }
        foreach (LeaderboardStruct row in leadearsArray)
        {
            GameObject obj = Instantiate(leaderBoardRow, mainPanel.gameObject.transform);
            obj.GetComponent<LeaderboardRow>().Setup(row.MyTime, row.MyName, row.MyPlace);
        }

    }

    void Start()
    {
        button.onClick.AddListener(GameFlow.Instance.GameRestart);
    }

    public void UpdateCurrentPlayer(string name)
    {
        playerName = name;
    }

    public class LeaderboardStruct
    {
        public float MyTime;
        public string MyName;
        public int MyPlace;

        public LeaderboardStruct(float time, string name)
        {
            MyTime = time;
            MyName = name;
        }
    }

}
