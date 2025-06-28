using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameFlow : MonoBehaviour
{
    public int number;
    private static GameFlow instance;
    private Leaderboard leaderboard;

    public List<Leaderboard.LeaderboardStruct> leaderboardSave = new List<Leaderboard.LeaderboardStruct>();

    public delegate void OnWin();
    public OnWin winEvent;
    public float currentTime;

    public static GameFlow Instance
    {

        get { return instance ?? (instance = new GameObject("Singleton").AddComponent<GameFlow>()); }
    }

    void Start()
    {
        DontDestroyOnLoad(instance);
    }

    public void GameRestart()
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Win()
    {
        winEvent?.Invoke();
        if (leaderboard == null)
        {
            GameObject prefab = Resources.Load<GameObject>("Prefab/Leaderboard");
            leaderboard = Instantiate(prefab).GetComponent<Leaderboard>();
            leaderboard.AddRow(currentTime);
        }
        else
        {
            leaderboard.AddRow(currentTime);
        }

        Time.timeScale = 0.0f;
        // SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}