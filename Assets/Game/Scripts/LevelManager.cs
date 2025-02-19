using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public Player player;

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
    }

    private void OnDisable()
    {
        EventBus.Instance.levelFinished -= LoadNextLevel;

    }

    private void Start()
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
        levelLoader.LoadLevel(++currentLevel - 1);
        player.transform.position = levelLoader.currentLevel.GetComponentInChildren<SpawnPoint>().transform.position;
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
