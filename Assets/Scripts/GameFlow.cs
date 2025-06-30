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

    public List<Leaderboard.LeaderboardStruct> leaderboardSave = new List<Leaderboard.LeaderboardStruct>();

    public delegate void OnWin();
    public OnWin winEvent;
    public double currentTime;
    [SerializeField]
    private PauseScreen pauseScreen;
    //private WinScreen winScreen;

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
        if (winScreen == null)
        {
            GameObject prefab = Resources.Load<GameObject>("Prefab/WinScreen");
            winScreen = Instantiate(prefab).GetComponent<WinScreen>();
            // winScreen.AddRow(currentTime);
        }
        else
        {
            // winScreen.AddRow(currentTime);
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
    }

    public void LoadLevel()
    {
        SceneManager.LoadScene("Lv2");
    }
}