using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameModeButton : MonoBehaviour
{
    public bool HiddenOnDisable;
    public string UnlockText;
    public int GMode;
    private bool _enabled;

    public void SetGameMode()
    {
        GameMode.Set(GMode);
    }

    void SetEnabled(bool enable)
    {
        if (enable)
        {
            _enabled = true;
            GetComponent<Button>().interactable = true;
            GetComponent<Image>().enabled = true;
            GetComponentInChildren<TextMeshProUGUI>().enabled = true;
            GetComponentInChildren<TextMeshProUGUI>().text = UnlockText;
        }
        else
        {
            _enabled = false;
            GetComponent<Button>().interactable = false;
            if (HiddenOnDisable)
            {
                GetComponent<Image>().enabled = false;
                GetComponentInChildren<TextMeshProUGUI>().enabled = false;
            }
            else
            {
                GetComponentInChildren<TextMeshProUGUI>().text = "Locked";
            }
        }
    }

    private void Update()
    {
        if (GMode > GameMode.GetMax())
        {
            SetEnabled(false);
        }
        else
        {
            SetEnabled(true);
        }

        if (_enabled)
        {
            if (GameMode.Get() == GMode)
            {
                GetComponent<Button>().interactable = false;
            }
            else
            {
                GetComponent<Button>().interactable = true;
            }
        }
        else
        {
            GetComponent<Button>().interactable = false;
        }
    }
}
