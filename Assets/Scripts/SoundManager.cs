using System.Collections.Generic;
using UnityEngine;

public class SoundManager : SingletonMonoBehaviour<SoundManager>
{
    [SerializeField] AudioSource bgmSource;

    public float BgmVolume
    {
        get
        {
            return bgmSource.volume;
        }
        set
        {
            var volume = Mathf.Clamp01(value);
            bgmSource.volume = volume;
        }
    }

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    public void PlayBgm()
    {
        bgmSource.Play();
    }

    public void StopBgm()
    {
        bgmSource.Stop();
    }
}