using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelMenuItem : MonoBehaviour
{
    public string LevelName;
    public int LevelID;
    public string Description;
    public GameObject Ranks;
    private LevelRanks _ranks;
    public TextMeshProUGUI Name;
    public TextMeshProUGUI Time;
    public TextMeshProUGUI Rank;
    private Button _button;


    // Start is called before the first frame update
    void Start()
    {
        _button = GetComponentInChildren<Button>();
        Name.text = LevelName;
        _ranks = Ranks.GetComponent<LevelRanks>();
        float score = ScoreTracker.GetScore(LevelID, GameMode.Get());
        if (score == 0f)
        {
            Time.text = "Incomplete";
            Rank.text = "";
        }
        else
        {
            Time.text = FloatToTime.Convert(ScoreTracker.GetScore(LevelID, GameMode.Get()));
            _ranks.SetRankIndicator(Rank, score);
        }
    }
    
    public void UpdateText()
    {
        float score = ScoreTracker.GetScore(LevelID, GameMode.Get());
        if (score == 0f)
        {
            Time.text = "Incomplete";
            Rank.text = "";
        }
        else
        {
            Time.text = FloatToTime.Convert(ScoreTracker.GetScore(LevelID, GameMode.Get()));
            _ranks.SetRankIndicator(Rank, score);
        }
    }

    public void LoadScene()
    {
        //SceneManager.LoadScene(LevelID);
        SelectLevel();
    }

    public void SelectLevel()
    {
        GameMode.SetID(LevelID);

        GameMode.SetDescription(Description);

        switch (_ranks.GetRankText(ScoreTracker.GetScore(LevelID, 0)))
        {
            case "S":
                GameMode.SetMax(5);
                break;
            case "A":
                GameMode.SetMax(4);
                break;
            case "B":
                GameMode.SetMax(3);
                break;
            case "C":
                GameMode.SetMax(2);
                break;
            case "D":
                GameMode.SetMax(1);
                break;
            default:
                GameMode.SetMax(0);
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        _button.interactable = GameMode.GetID() != LevelID;
        
        UpdateText();
    }
}
