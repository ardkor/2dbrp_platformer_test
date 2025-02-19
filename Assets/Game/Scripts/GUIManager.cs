using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GUIManager: MonoBehaviour
{
    [SerializeField] private Slider _hpSlider;

    [SerializeField] private GameObject _restartPanel;

    public void SetHpSliderValue(int Hp, int maxHp)
    {
        _hpSlider.value = ((float)Hp) / maxHp;
    }
    private void ActivateRestartPanel()
    {
        _restartPanel.SetActive(true);
    }
    private void OnEnable()
    {
        EventBus.Instance.playerDamaged += SetHpSliderValue;
        EventBus.Instance.playerDied += ActivateRestartPanel;
        EventBus.Instance.gameFinished += ActivateRestartPanel;
    }
    private void OnDisable()
    {
        EventBus.Instance.playerDamaged -= SetHpSliderValue;
        EventBus.Instance.playerDied -= ActivateRestartPanel;
        EventBus.Instance.gameFinished -= ActivateRestartPanel;
    }
}
