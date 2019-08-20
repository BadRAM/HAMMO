using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelMenuItem : MonoBehaviour
{
    public string LevelName;
    public int LevelID;
    public GameObject Ranks;
    private LevelRanks _ranks;
    public TextMeshProUGUI Name;
    public TextMeshProUGUI Time;
    public TextMeshProUGUI Rank;

    // Start is called before the first frame update
    void Start()
    {
        Name.text = LevelName;
        _ranks = Ranks.GetComponent<LevelRanks>();
        float score = ScoreTracker.GetScore(LevelID);
        if (score == 0f)
        {
            Time.text = "Incomplete";
            Rank.text = "";
        }
        else
        {
            Time.text = FloatToTime.Convert(ScoreTracker.GetScore(LevelID));
            _ranks.SetRankIndicator(Rank, score);
        }
    }

    public void LoadScene()
    {
        SceneManager.LoadScene(LevelID);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
