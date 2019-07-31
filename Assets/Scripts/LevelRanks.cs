using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelRanks : MonoBehaviour
{
    public float SRank;
    public Color SColor;
    public float ARank;
    public Color AColor;
    public float BRank;
    public Color BColor;
    public float CRank;
    public Color CColor;
    public Color DColor;

    string FloatToTime(float time)
    {
        int minutes = (int)time / 60;
        int seconds = (int)time - 60 * minutes;
        int milliseconds = (int)(1000 * (time - minutes * 60 - seconds));
        return string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, milliseconds);
    }

    public void SetRankIndicator(TextMeshProUGUI target, float time)
    {
        if (time < SRank)
        {
            target.text = "S";
            target.color = SColor;
        }
        else if (time < ARank)
        {
            target.text = "A";
            target.color = AColor;
        }
        else if (time < BRank)
        {
            target.text = "B";
            target.color = BColor;
        }
        else if (time < CRank)
        {
            target.text = "C";
            target.color = CColor;
        }
        else
        {
            target.text = "D";
            target.color = DColor;
        }
    }

    public string GetRankPreview(float time)
    {
        // only show S rank time if A rank has been achieved
        if (time < ARank)
        {
            return "S: " + FloatToTime(SRank) + "\nA: " + FloatToTime(ARank) + "\nB: " + FloatToTime(BRank) + "\nC: " + FloatToTime(CRank);
        }
        else
        {
            return "A: " + FloatToTime(ARank) + "\nB: " + FloatToTime(BRank) + "\nC: " + FloatToTime(CRank);
        }
    }
}
