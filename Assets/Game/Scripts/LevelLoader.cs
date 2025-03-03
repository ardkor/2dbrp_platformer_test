using UnityEngine;
public class LevelLoader : MonoBehaviour
{
    [SerializeField] private GameObject[] levels;
    public GameObject currentLevel { get; private set; }

    public void LoadLevel(int index)
    {
        if (currentLevel != null)
        {
            Destroy(currentLevel);
        }
        if (index >= 0 && index < levels.Length)
        {
            currentLevel = Instantiate(levels[index], levels[index].transform.position, Quaternion.identity);
        }
    }
}