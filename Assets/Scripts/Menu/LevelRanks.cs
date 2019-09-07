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

    public string GetRankText(float time)
    {
        if (time == 0f)
        {
            return "";
        }
        else if (time < SRank)
        {
            return "S";
        }
        else if (time < ARank)
        {
            return "A";
        }
        else if (time < BRank)
        {
            return "B";
        }
        else if (time < CRank)
        {
            return "C";
        }
        else
        {
            return "D";
        }
    }

    public string GetRankPreview(float time)
    {
        // only show S rank time if A rank has been achieved
        if (time < ARank && time > 0f)
        {
            return "S: " + FloatToTime.Convert(SRank) + "\nA: " + FloatToTime.Convert(ARank) + "\nB: " + FloatToTime.Convert(BRank) + "\nC: " + FloatToTime.Convert(CRank);
        }
        else
        {
            return "A: " + FloatToTime.Convert(ARank) + "\nB: " + FloatToTime.Convert(BRank) + "\nC: " + FloatToTime.Convert(CRank);
        }
    }
}
