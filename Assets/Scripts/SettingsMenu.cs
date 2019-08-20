using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class SettingsMenu : MonoBehaviour
{
    public TMP_InputField Sensitivity;
    
    public void ApplySettings()
    {
        SettingsManager.SaveSensitivity(float.Parse(Sensitivity.text));
    }
    
    public void ResetSensitivityField()
    { 
        Sensitivity.text = SettingsManager.LoadSensitivity().ToString();
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
