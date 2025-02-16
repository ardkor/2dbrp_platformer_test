using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public Player player;

    public GameObject[] levels;

    public LevelLoader levelLoader;
    private int currentLevel;

    private void Awake()
    {
        currentLevel = 1;
        LoadNextLevel();
    }

    private void LoadNextLevel()
    {
        levelLoader.LoadLevel(currentLevel - 1);
        player.transform.position = levelLoader.currentLevel.GetComponentInChildren<SpawnPoint>().transform.position;
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
