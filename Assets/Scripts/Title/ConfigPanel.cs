using UnityEngine;
using UnityEngine.UI;

public sealed class ConfigPanel : MonoBehaviour
{
    [SerializeField] Slider slider;

    public void OnValueChanged()
    {
        SoundManager.Instance.BgmVolume = slider.value;
    }

    public void OnClickClose()
    {
        gameObject.SetActive(false);
    }
}
