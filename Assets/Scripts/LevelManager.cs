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
        currentLevel = 1;
        //LoadNextLevel();
    }
    private void Start()
    {
        SoundManager.Instance.StartMusic(SoundManager.menuMusic);
    }

    private void LoadNextLevel()
    {
        SoundManager.Instance.PlaySound(SoundManager.teleportSound);
        levelLoader.LoadLevel(currentLevel - 1);
        player.transform.position = levelLoader.currentLevel.GetComponentInChildren<SpawnPoint>().transform.position;
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
