using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class SettingsManager
{
    public static Settings CurrentSettings { get; private set; }

    static SettingsManager()
    {
        LoadSettings();
    }
    
    public static void LoadSettings()
    {
        if (File.Exists(Application.dataPath + "/settings.sav"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.dataPath + "/settings.sav", FileMode.Open);
            CurrentSettings = (Settings)bf.Deserialize(file);
            file.Close();
        }
        else
        {
            // create default settings.
            CurrentSettings = new Settings();
            CurrentSettings.Res = new int[]
            {
                Screen.resolutions[Screen.resolutions.Length - 1].width,
                Screen.resolutions[Screen.resolutions.Length - 1].height
            };
        }
    }

    public static void SaveSettings()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.dataPath + "/settings.sav");
        bf.Serialize(file, CurrentSettings);
        file.Close();
    }

    public static void SaveSettings(Settings newSettings)
    {
        CurrentSettings = newSettings;
        SaveSettings();
    }

    public static void ApplyResolution()
    {
        Screen.SetResolution(CurrentSettings.Res[0], CurrentSettings.Res[1], CurrentSettings.FullScreen);
    }
}