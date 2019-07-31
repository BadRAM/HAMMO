﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreIndicator : MonoBehaviour
{
    public int levelID;
    public TextMeshProUGUI textMesh;
    public LevelRanks LevelRanks;
    public TextMeshProUGUI RankText;

    public void Start()
    {
        UpdateText();
    }

    public void UpdateText()
    {
        float score = ScoreTracker.GetScore(levelID);
        if (score == 0f)
        {
            textMesh.text = "Incomplete";
            RankText.text = "";
        }
        else
        {
            textMesh.text = FloatToTime(ScoreTracker.GetScore(levelID));
            LevelRanks.SetRankIndicator(RankText, score);
        }
    }

    string FloatToTime(float time)
    {
        int minutes = (int)time / 60;
        int seconds = (int)time - 60 * minutes;
        int milliseconds = (int)(1000 * (time - minutes * 60 - seconds));
        return string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, milliseconds);
    }
}
