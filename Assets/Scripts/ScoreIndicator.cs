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
            textMesh.text = FloatToTime.Convert(ScoreTracker.GetScore(levelID));
            LevelRanks.SetRankIndicator(RankText, score);
        }
    }
}
