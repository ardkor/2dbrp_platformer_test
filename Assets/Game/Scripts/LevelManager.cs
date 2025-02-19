using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public Player player;
    public Camera camera;

    public LevelLoader levelLoader;
    private int currentLevel;

    private void Awake()
    {
        currentLevel = 0;
        LoadNextLevel();
    }

    private void OnEnable()
    {
        EventBus.Instance.levelFinished += LoadNextLevel;
        EventBus.Instance.playerDied += PlayMenuMusic;
    }

    private void OnDisable()
    {
        EventBus.Instance.levelFinished -= LoadNextLevel;
        EventBus.Instance.playerDied -= PlayMenuMusic;

    }

    private void Start()
    {
        SoundInstance.musicVolume = 0.7f;
        SoundManager.Instance.StartMusic(SoundManager.firstLevelMusic);
    }
    private void PlayMenuMusic()
    {
        SoundManager.Instance.StartMusic(SoundManager.menuMusic);
    }
    private void LoadNextLevel()
    {
        if(currentLevel == 2)
        {
            EventBus.Instance.gameFinished?.Invoke();
            return;
        }
        SoundManager.Instance.PlaySound(SoundManager.teleportSound);
        SoundManager.Instance.StartMusic(SoundManager.secondLevelMusic);
        levelLoader.LoadLevel(++currentLevel - 1);
        player.transform.position = levelLoader.currentLevel.GetComponentInChildren<SpawnPoint>().transform.position;
        camera.transform.position = player.transform.position;
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
