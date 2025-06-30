using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class GameFlow : MonoBehaviour
{
    public int number;
    private static GameFlow instance;
    private WinScreen winScreen;

    public List<Leaderboard.LeaderboardStruct> leaderboardSave = new List<Leaderboard.LeaderboardStruct>()
    { new Leaderboard.LeaderboardStruct("01:00:00", "Daren"), new Leaderboard.LeaderboardStruct("02:00:00", "Romu"), new Leaderboard.LeaderboardStruct("03:00:00", "Yaroslave"),
    new Leaderboard.LeaderboardStruct("04:00:00", "Egor"), new Leaderboard.LeaderboardStruct("05:00:00", "Vlad")};

    public delegate void OnWin();
    public OnWin winEvent;
    public double currentTime;
    [SerializeField]
    private PauseScreen pauseScreen;
    public MusicPlayer musicPlayer;
    //private WinScreen winScreen;

    public static GameFlow Instance
    {

        get { return instance ?? (instance = new GameObject("Singleton").AddComponent<GameFlow>()); }
    }

    void Start()
    {
        DontDestroyOnLoad(instance);
        GameObject musicPrefab = Resources.Load<GameObject>("Prefab/MusicPlayer");
        musicPlayer = Instantiate(musicPrefab).GetComponent<MusicPlayer>();
        DontDestroyOnLoad(musicPlayer.gameObject);
        // musicPlayer.ChangeMusic("Lv1", 0);  
    }
    public void Crutch()
    {

    }

    public void GameRestart()
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Win()
    {
        winEvent?.Invoke();
        if (winScreen == null)
        {
            GameObject prefab = Resources.Load<GameObject>("Prefab/WinScreen");
            winScreen = Instantiate(prefab).GetComponent<WinScreen>();
        }
        else
        {
        }

        TogglePause();
        // SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void TogglePause()
    {
        if (Time.timeScale == 0.0f)
        {
            Time.timeScale = 1.0f;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Time.timeScale = 0.0f;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    public void CallPauseMenu()
    {
        if (Time.timeScale == 0.0f)
        {
            pauseScreen.GoBack();
        }
        else
        {
            GameObject prefab = Resources.Load<GameObject>("Prefab/PauseScreen");
            pauseScreen = Instantiate(prefab).GetComponent<PauseScreen>();
        }
        TogglePause();

    }

    public void Return()
    {
        TogglePause();
        SceneManager.LoadScene("Lv1");
        musicPlayer.ChangeMusic("Lv1", 1);
    }

    public void LoadLevel()
    {
        SceneManager.LoadScene("Lv2");
        musicPlayer.ChangeMusic("Lv2", 1);
    }

    public void ChangeVolume(float volume)
    {
        musicPlayer.ChangeVolume(volume); 
    }
}