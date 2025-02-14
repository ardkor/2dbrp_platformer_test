using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GUIManager: MonoBehaviour
{
    [SerializeField] private Slider _hpSlider;

    public void SetHpSliderValue(int Hp, int maxHp)
    {
        _hpSlider.value = ((float)Hp) / maxHp;
    }

    private void OnEnable()
    {
        EventBus.Instance.PlayerDamaged += SetHpSliderValue;
    }
    private void OnDisable()
    {
        EventBus.Instance.PlayerDamaged -= SetHpSliderValue;
    }
}
