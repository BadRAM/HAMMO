using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MainMenu : MonoBehaviour
{
    public Canvas TopMenuCanvas;
    public Canvas LevelSelectCanvas;
    public Canvas SettingsCanvas;
    public AudioMixer Mixer;

    // Start is called before the first frame update
    void Start()
    {
        Mixer.SetFloat("MusicVol",-Mathf.Pow((-SettingsManager.CurrentSettings.MusicVolume + 1) * 8, 2));
        Mixer.SetFloat("SFXVol", -Mathf.Pow((-SettingsManager.CurrentSettings.SFXVolume + 1) * 8, 2));
        TopMenuCanvas.enabled = true;
        LevelSelectCanvas.enabled = false;
        SettingsCanvas.enabled = false;
    }

    public void TopMenu()
    {
        TopMenuCanvas.enabled = true;
        LevelSelectCanvas.enabled = false;
        SettingsCanvas.enabled = false;
    }

    public void LevelSelect()
    {
        GameMode.SetID(-1);
        GameMode.SetMax(0);
        TopMenuCanvas.enabled = false;
        LevelSelectCanvas.enabled = true;
        SettingsCanvas.enabled = false;
    }

    public void Settings()
    {
        SettingsCanvas.GetComponent<SettingsMenu>().ResetFields();
        TopMenuCanvas.enabled = false;
        LevelSelectCanvas.enabled = false;
        SettingsCanvas.enabled = true;
    }

}
