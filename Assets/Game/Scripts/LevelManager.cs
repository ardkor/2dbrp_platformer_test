using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private Transform _camera;
    [SerializeField] private LevelLoader levelLoader;

    private int currentLevel;

    private void Awake()
    {
        
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
        currentLevel = 0;
        LoadNextLevel();
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
        //_camera = c.transform;
        levelLoader.currentLevel.GetComponentInChildren<ParallaxEffect>().Initialize();
        player.transform.position = levelLoader.currentLevel.GetComponentInChildren<SpawnPoint>().transform.position;
        Camera.main.transform.position = player.transform.position;
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
