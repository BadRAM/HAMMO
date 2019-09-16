using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Settings
{
    public float Sensitivity = 2f;
    public float SFXVolume = 1f;
    public float MusicVolume = 1f;
    public bool FullScreen = true;
    public int[] Res; // Uncommenting this crashed the unity editor on play, when the variable was named Resolution
}
