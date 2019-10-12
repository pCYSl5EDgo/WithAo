using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConfigPanel : MonoBehaviour
{
    [SerializeField] Slider slider;

    public void OnValueChanged()
    {
        SoundManager.Instance.BgmVolume = slider.value;
    }
}
