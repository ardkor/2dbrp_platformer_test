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
        EventBus.Instance.PlayerDamaged += SetHpSliderValue;
        EventBus.Instance.PlayerDied += ActivateRestartPanel;
    }
    private void OnDisable()
    {
        EventBus.Instance.PlayerDamaged -= SetHpSliderValue;
        EventBus.Instance.PlayerDied -= ActivateRestartPanel;
    }
}
