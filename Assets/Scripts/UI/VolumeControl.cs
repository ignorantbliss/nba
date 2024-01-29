using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolumeControl : MonoBehaviour
{
    public AudioSource ToControl;
    public string SettingName = "Volume";
    public UnityEngine.UI.Slider Slider;

    public void Start()
    {
        ToControl.volume = PlayerPrefs.GetFloat(SettingName,1);
        Slider.value = ToControl.volume * 100;
    }

    public void UpdateVolume(float V)
    {
        ToControl.volume = V / 100f;
        PlayerPrefs.SetFloat(SettingName, ToControl.volume);
    }
}
