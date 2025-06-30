using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseScreen : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private Button buttonRestart;
    [SerializeField]
    private Button buttonPause;
    [SerializeField]
    private Button buttonQuit;
    [SerializeField]
    private GameObject mainPanel;

    [SerializeField]
    private Slider volumeSlider;
    [SerializeField]
    private Slider sensSlider;

    void Start()
    {
        buttonRestart.onClick.AddListener(GameFlow.Instance.GameRestart);
        buttonPause.onClick.AddListener(GameFlow.Instance.CallPauseMenu);
        buttonQuit.onClick.AddListener(GameFlow.Instance.Return);
        volumeSlider.onValueChanged.AddListener(OnSliderVolumeChange);
        sensSlider.onValueChanged.AddListener(OnSliderSensChange);
        volumeSlider.value = GameFlow.Instance.musicPlayer.baseVolume;
        mainPanel.transform.position = new Vector3(2560 + 1280, 720, 0);
        PlayAnimation(false);
        sensSlider.value = GameFlow.Instance.baseSens;
    }

    void OnSliderVolumeChange(float value)
    {
        GameFlow.Instance.musicPlayer.ChangeVolume(value);
    }
    void OnSliderSensChange(float value)
    {
        GameFlow.Instance.ChangeSens(value);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void GoBack()
    {
        PlayAnimation(true);
        Destroy(this);
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
