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
    private float playerTime = 0.0f;
    public float MyTime
    {
        get => playerTime;
        private set
        {
            playerTime = value;
            timeLabel.text = $"{(int)(playerTime / 60):00}:{(int)(playerTime % 60):00}";
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

    public void Setup(float time, string name, int place)
    {
        MyTime = time;
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
