using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Slider = UnityEngine.UI.Slider;

public class SettingsMenu : MonoBehaviour
{
    public TMP_InputField Sensitivity;
    public Slider MusicVolume;
    public Slider SFXVolume;
    public AudioMixer Mixer;
    public AnimationCurve LinearToDecibel;

    public void ApplySettings()
    {
        float musicVol = -Mathf.Pow((-MusicVolume.value + 1) * 8, 2);
        //Debug.Log("musicvol: " + musicVol + ", value: " + MusicVolume.value);
        float sfxVol = -Mathf.Pow((-SFXVolume.value + 1) * 8, 2);
        Mixer.SetFloat("MusicVol", musicVol);
        Mixer.SetFloat("SFXVol", sfxVol);
        
        Settings s = new Settings();
        s.Sensitivity = float.Parse(Sensitivity.text);
        s.MusicVolume = MusicVolume.value;
        s.SFXVolume = SFXVolume.value;
        SettingsManager.SaveSettings(s);
    }
    
    public void ResetFields()
    {
        Sensitivity.text = SettingsManager.CurrentSettings.Sensitivity.ToString();
        MusicVolume.value = SettingsManager.CurrentSettings.MusicVolume;
        SFXVolume.value = SettingsManager.CurrentSettings.SFXVolume;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
