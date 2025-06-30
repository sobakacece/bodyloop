using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    [SerializeField]
    private AudioSource audioSource;
    public float baseVolume;
    [SerializeField]
    public MusicLocationStruct[] musicList;

    public void Start()
    {
        audioSource.volume = baseVolume;

    }
    public void ChangeMusic(string sceneName, float fadeTime = 1f)
    {
        StopAllCoroutines();
        StartCoroutine(FadeToNewClip(sceneName, fadeTime));
    }

    private IEnumerator FadeToNewClip(string sceneName, float fadeTime)
    {
        AudioClip newClip = FindItem(sceneName);
        if (newClip == null) yield return null;
        float startVolume = baseVolume;

        for (float t = 0; t < fadeTime; t += Time.unscaledDeltaTime)
        {
            audioSource.volume = Mathf.Lerp(startVolume, 0f, t / fadeTime);
            yield return null;
        }

        audioSource.clip = newClip;
        audioSource.Play();

        for (float t = 0; t < fadeTime; t += Time.unscaledDeltaTime)
        {
            audioSource.volume = Mathf.Lerp(0f, startVolume, t / fadeTime);
            yield return null;
        }

        audioSource.volume = baseVolume;
    }

    private AudioClip FindItem(string name)
    {
        Debug.Log(name);
        foreach (MusicLocationStruct musicStruct in musicList)
        {
            if (musicStruct.sceneName == name)
            {
                return musicStruct.track;
            }
        }
        return null;
    }

    public void ChangeVolume(float volume)
    {
        baseVolume = volume;
        audioSource.volume = baseVolume;
    }
    [System.Serializable]
    public class MusicLocationStruct
    {
        public string sceneName;
        public AudioClip track;
    }
}


