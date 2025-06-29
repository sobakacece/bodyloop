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
    private Button buttonPause;
    [SerializeField]
    private Button buttonQuit;

    void Start()
    {
        buttonRestart.onClick.AddListener(GameFlow.Instance.GameRestart);
        buttonRestart.onClick.AddListener(GameFlow.Instance.Pause);
        buttonRestart.onClick.AddListener(GameFlow.Instance.Return);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void GoBack()
    {
        GameFlow.Instance.Pause();
        Destroy(this);
    }
}
