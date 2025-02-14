using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public Player player;
    public Transform firstLevelSpawn;
    public Transform secondLevelSpawn;

    public GameObject[] levels;

    public LevelLoader levelLoader;

    private void Awake()
    {
        levelLoader.LoadLevel(0);
    }
   
    private void LoadSecondLevel()
    {
        player.transform.position = firstLevelSpawn.position;
    }
    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
