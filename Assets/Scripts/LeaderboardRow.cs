using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardRow : MonoBehaviour
{
    [SerializeField]
    private Text timeLabel;
    [SerializeField]
    private Text placeLable;
    [SerializeField]
    private Text nameLabel;
    private string playerTime = "00:00:000";
    public string MyTextTime
    {
        get => playerTime;
        private set
        {
            playerTime = value;
            timeLabel.text = playerTime;
        }
    }
    private string playerName = "Daren";
    public string MyName
    {
        get => playerName;
        private set
        {
            playerName = value;
            nameLabel.text = playerName;
        }
    }
    private int playerPlace;
        public int MyPlace
    {
        get => playerPlace;
        set
        {
            playerPlace = value;
            placeLable.text = playerPlace.ToString();
        }
    }

    public void Setup(string time, string name, int place)
    {
        MyTextTime = time;
        MyName = name;
        MyPlace = place;   
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
