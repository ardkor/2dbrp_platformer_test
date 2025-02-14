using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public const string deathSound = "death";
    public const string jumpSound = "jump";
    public const string accelerateSound = "accelerate";

    public const string firstLevelMusic = "slow";
    public const string secondLevelMusic = "action";
    private const string menuMusic = "chill";

    private SoundManager() { }

    private static SoundManager _instance;

    public static SoundManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = new SoundManager();
            return _instance;
        }
    }

    private void Start()
    {
        StartMusic(menuMusic);
    }

    public void PlaySound(string soundName)
    {
        //                                   AudioClip  Transform Volume Is3D   Randomization
        //SoundInstance.InstantiateOnTransform(Clip_Fire, transform, -1, false, SoundInstance.Randomization.Medium);
        // Playing from the library would be like this:
        SoundInstance.InstantiateOnTransform(SoundInstance.GetClipFromLibrary(soundName), transform, 1.0f, false, SoundInstance.Randomization.Medium);
    }

    public void StartMusic(string musicName)
    {
        SoundInstance.StartMusic(musicName, 0.7f);
    }

    public void SwitchMusic()
    {
        SoundInstance.StartMusic(SoundInstance.GetNextMusic().name, 1f);
    }

    public void PauseMusic()
    {
        SoundInstance.PauseMusic(1.5f);
    }

    public void ResumeMusic()
    {
        SoundInstance.ResumeMusic(1.5f);
    }
}
