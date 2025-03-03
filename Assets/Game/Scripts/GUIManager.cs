using UnityEngine;
using UnityEngine.UI;

public class GUIManager: MonoBehaviour
{
    [SerializeField] private Slider _hpSlider;

    [SerializeField] private GameObject _restartPanel;
    [SerializeField] private GameObject _winText;

    private void SetHpSliderValue(int Hp, int maxHp)
    {
        _hpSlider.value = ((float)Hp) / maxHp;
    }
    private void ActivateRestartPanel()
    {
        _restartPanel.SetActive(true);
    }
    private void ActivateWinPanel()
    {
        _restartPanel.SetActive(true);
        _winText.SetActive(true);
    }
    private void OnEnable()
    {
        EventBus.Instance.playerDamaged += SetHpSliderValue;
        EventBus.Instance.playerDied += ActivateRestartPanel;
        EventBus.Instance.gameFinished += ActivateWinPanel;
    }
    private void OnDisable()
    {
        EventBus.Instance.playerDamaged -= SetHpSliderValue;
        EventBus.Instance.playerDied -= ActivateRestartPanel;
        EventBus.Instance.gameFinished -= ActivateWinPanel;
    }
}
