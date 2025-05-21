using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class StageResult
{
    public string playername;
    public int stage;
    public int score;
}
[System.Serializable]
public class StageReultList
{
    public List<StageResult> reuslt = new List<StageResult>();
}
public static class StageResultsever
{
    private const string FILE        = "stage_reuslt.json";
    private const string PLAYER_NAME = "Plyer_Name";
    private static string filePath = Path.Combine(Application.persistentDataPath, FILE);

    public static void SaveStage(int stage, int score)
    {
        StageReultList list = LoadInternal();
        string plyerName = PlayerPrefs.GetString(PLAYER_NAME, "");
        StageResult enpty = new StageResult()
        {
            playername = plyerName,
            stage = stage,
            score = score
        };
        list.reuslt.Add(enpty);
        string json = JsonUtility.ToJson(list, true);
        File.WriteAllText(filePath, json);

       
    }

    public static StageReultList     LoadRank()
    {
        return LoadInternal();
    }
    private static StageReultList LoadInternal()
    {
        if (!File.Exists(filePath))
            return new StageReultList();
        string json = File.ReadAllText(filePath);
        StageReultList list = JsonUtility.FromJson<StageReultList>(json);
        if (list == null)
            return new StageReultList();
        else
            return list;
    }
}