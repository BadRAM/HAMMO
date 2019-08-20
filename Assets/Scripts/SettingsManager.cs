using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class SettingsManager
{
    private static Settings _settings;

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
            _settings = (Settings)bf.Deserialize(file);
            file.Close();
        }
        else
        {
            // create default settings.
            _settings = new Settings();
            _settings.Sensitivity = 2;
        }
    }

    public static void SaveSettings()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.dataPath + "/settings.sav");
        bf.Serialize(file, _settings);
        file.Close();
    }
    
    public static float LoadSensitivity()
    {
        return _settings.Sensitivity;
    }

    public static void SaveSensitivity(float sensitivity)
    {
        _settings.Sensitivity = sensitivity;
        SaveSettings();
    }
}