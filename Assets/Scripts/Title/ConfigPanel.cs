using UnityEngine;
using UnityEngine.UI;
using Zenject;

public sealed class ConfigPanel : MonoBehaviour
{
    [Inject] private SoundManager soundManager;

    [SerializeField] Slider slider;

    private void OnEnable()
    {
        slider.value = soundManager.BgmVolume;
    }

    public void OnValueChanged()
    {
        soundManager.BgmVolume = slider.value;
    }

    public void OnClickClose()
    {
        gameObject.SetActive(false);
    }
}
