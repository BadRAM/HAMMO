using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class ScoreTracker
{
    private static IDictionary<string, float> _scores = new Dictionary<string, float>();

    static ScoreTracker()
    {
        LoadScores();
    }

    public static void AddScore(int levelID, int gameMode, float score)
    {
        // inefficient to read scores every save, could be changed to game start
        LoadScores();

        string key = levelID + "," + gameMode;
        
        if (_scores.ContainsKey(key))
        {
            float oldscore;
            _scores.TryGetValue(key, out oldscore);
            if (score < oldscore)
            {
                _scores.Remove(key);
                _scores.Add(key, score);
                SaveScores();
            }
        }
        else
        {
            _scores.Add(key, score);
            SaveScores();
        }
    }

    public static float GetScore(int levelID, int gameMode)
    {
        string key = levelID + "," + gameMode;

        //LoadScores(); // not sure why this was here, pretty inefficient to access a file for no reason.
        float score = 0f;
        if (_scores.ContainsKey(key))
        {
            _scores.TryGetValue(key, out score);
        }
        return score;
    }

    private static void SaveScores()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.dataPath + "/times.sav");
        bf.Serialize(file, _scores);
        file.Close();
    }

    private static void LoadScores()
    {
        if (File.Exists(Application.dataPath + "/times.sav"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.dataPath + "/times.sav", FileMode.Open);
            IDictionary<string, float> scores = (IDictionary<string, float>)bf.Deserialize(file);
            /*
            foreach (var i in scores)
            {
                Debug.Log(i);
            }
            */
            file.Close();

            _scores = scores;
        }
        else
        {
            _scores = new Dictionary<string, float>();
        }
    }
}
