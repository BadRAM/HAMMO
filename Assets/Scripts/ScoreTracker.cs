using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class ScoreTracker
{
    private static IDictionary<int, float> _scores = new Dictionary<int, float>();

    public static void AddScore(int levelID, float score)
    {
        // inefficient to read scores every save, could be changed to game start
        LoadScores();
        
        if (_scores.ContainsKey(levelID))
        {
            float oldscore;
            _scores.TryGetValue(levelID, out oldscore);
            if (score < oldscore)
            {
                _scores.Remove(levelID);
                _scores.Add(levelID, score);
                SaveScores();
            }
        }
        else
        {
            _scores.Add(levelID, score);
            SaveScores();
        }
    }

    public static float GetScore(int levelID)
    {
        LoadScores();
        float score = 0f;
        if (_scores.ContainsKey(levelID))
        {
            _scores.TryGetValue(levelID, out score);
        }
        return score;
    }

    public static void SaveScores()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.dataPath + "/times.sav");
        bf.Serialize(file, _scores);
        file.Close();
    }

    public static void LoadScores()
    {
        if (File.Exists(Application.dataPath + "/times.sav"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.dataPath + "/times.sav", FileMode.Open);
            IDictionary<int, float> scores = (IDictionary<int, float>)bf.Deserialize(file);
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
            _scores = new Dictionary<int, float>();
        }
    }
}
