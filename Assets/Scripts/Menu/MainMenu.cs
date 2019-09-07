using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public Canvas TopMenuCanvas;
    public Canvas LevelSelectCanvas;
    public Canvas SettingsCanvas;

    // Start is called before the first frame update
    void Start()
    {
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
        TopMenuCanvas.enabled = false;
        LevelSelectCanvas.enabled = false;
        SettingsCanvas.enabled = true;
    }

}
