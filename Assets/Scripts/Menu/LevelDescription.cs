using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelDescription : MonoBehaviour
{
    public TextMeshProUGUI Text;

    private void Update()
    {
        Text.text = GameMode.GetDescription();
    }
}
