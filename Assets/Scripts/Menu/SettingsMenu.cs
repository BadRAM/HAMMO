using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Slider = UnityEngine.UI.Slider;
using Toggle = UnityEngine.UI.Toggle;

public class SettingsMenu : MonoBehaviour
{
    public TMP_InputField Sensitivity;
    public Slider MusicVolume;
    public Slider SFXVolume;
    public AudioMixer Mixer;
    public Toggle FullScreen;
    public TMP_Dropdown Resolution;
    
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
        s.FullScreen = FullScreen.isOn;
        s.Res = new int[]
        {
            stringToWidth(Resolution.options[Resolution.value].text),
            stringToHeight(Resolution.options[Resolution.value].text)
        };
        SettingsManager.SaveSettings(s);
        SettingsManager.ApplyResolution();
    }
    
    public void ResetFields()
    {
        Sensitivity.text = SettingsManager.CurrentSettings.Sensitivity.ToString();
        MusicVolume.value = SettingsManager.CurrentSettings.MusicVolume;
        SFXVolume.value = SettingsManager.CurrentSettings.SFXVolume;
        FullScreen.isOn = Screen.fullScreen;
        List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();
        int value = 0;
        foreach (Resolution res in Screen.resolutions)
        {
            if (options.Count == 0 ||
                res.width != stringToWidth(options[0].text) || 
                res.height != stringToHeight(options[0].text))
            {
                options.Insert(0, new TMP_Dropdown.OptionData(resToString(res)));
                if (res.height == Screen.height && res.width == Screen.width)
                {
                    value = 0;
                }
                else
                {
                    value++;
                }
            }
        }
        Resolution.options = options;
        Resolution.value = value;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private string resToString(Resolution res)
    {
        return res.width + "x" + res.height;
    }

    private int stringToWidth(string str)
    {
        return int.Parse(str.Split('x')[0]);
    }
    
    private int stringToHeight(string str)
    {
        return int.Parse(str.Split('x')[1]);
    }
}
