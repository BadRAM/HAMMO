using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreIndicator : MonoBehaviour
{
    public TextMeshProUGUI Level1ScoreText;
    public TextMeshProUGUI Level2ScoreText;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Level1ScoreText.text = FloatToTime(ScoreTracker.Level1Score);
        Level2ScoreText.text = FloatToTime(ScoreTracker.Level2Score);
    }

    string FloatToTime(float time)
    {
        int minutes = (int)time / 60;
        int seconds = (int)time - 60 * minutes;
        int milliseconds = (int)(1000 * (time - minutes * 60 - seconds));
        return string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, milliseconds);
    }
}
