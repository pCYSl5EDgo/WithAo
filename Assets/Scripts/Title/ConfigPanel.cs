using UnityEngine;
using UnityEngine.UI;

public sealed class ConfigPanel : MonoBehaviour
{
    [SerializeField] Slider slider;

    private void OnEnable()
    {
        Debug.Log($"元の音量：{SoundManager.Instance.BgmVolume}");
        slider.value = SoundManager.Instance.BgmVolume;
    }

    public void OnValueChanged()
    {
        Debug.Log($"変更：{slider.value}");
        SoundManager.Instance.BgmVolume = slider.value;
    }

    public void OnClickClose()
    {
        gameObject.SetActive(false);
    }
}
