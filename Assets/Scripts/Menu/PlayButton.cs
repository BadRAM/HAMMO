using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayButton : MonoBehaviour
{
    private Button _button;

    public void PlaySelectedLevel()
    {
        int id = GameMode.GetID();
        
        if (id != -1)
        {
            SceneManager.LoadSceneAsync(id);
        }
    }

    private void Start()
    {
        _button = GetComponent<Button>();
    }

    private void Update()
    {
        _button.interactable = GameMode.GetID() != -1;
    }
}
