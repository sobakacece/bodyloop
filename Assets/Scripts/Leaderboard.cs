using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;

public class Leaderboard : MonoBehaviour
{
    private List<LeaderboardStruct> leadearsArray = new List<LeaderboardStruct>();
    [SerializeField]
    private int maxLeaders = 5;
    [SerializeField]
    private VerticalLayoutGroup leaderboardPanel;
    [SerializeField]
    private VerticalLayoutGroup mainPanel;
    [SerializeField]
    private GameObject leaderBoardRow;
    private string playerName = "Player1";
    [SerializeField]
    private Button button;

    [SerializeField]
    // Start is called before the first frame update
    public void AddRow(string time)
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

    public void Appear()
    {
        mainPanel.transform.LeanMoveLocal(new Vector2(0, 0), 1).setEaseOutQuart().setIgnoreTimeScale(true);
        gameObject.SetActive(true);
    }
    public void Hide()
    {
        LTDescr hideAnim = mainPanel.transform.LeanMoveLocal(new Vector2(0, -451), 1).setEaseOutQuart().setIgnoreTimeScale(true);
        StartCoroutine(TurnOff(hideAnim.time));
    }

    IEnumerator TurnOff(float seconds)
    {
        yield return new WaitForSecondsRealtime(seconds);
        Destroy(gameObject);
    }
    void FillTable()
    {
        foreach (Transform child in leaderboardPanel.gameObject.transform)
        {
            Destroy(child.gameObject);
        }
        foreach (LeaderboardStruct row in leadearsArray)
        {
            Debug.Log(row.MyTime + row.MyName + row.MyPlace);
            GameObject obj = Instantiate(leaderBoardRow, leaderboardPanel.gameObject.transform);
            obj.GetComponent<LeaderboardRow>().Setup(row.MyTime, row.MyName, row.MyPlace);
        }

    }

    void Start()
    {
        button.onClick.AddListener(Hide);
        //Hide();
    }

    public void UpdateCurrentPlayer(string name)
    {
        playerName = name;
    }

    public class LeaderboardStruct
    {
        public string MyTime;
        public string MyName;
        public int MyPlace;

        public LeaderboardStruct(string time, string name)
        {
            MyTime = time;
            MyName = name;
        }
    }

}
