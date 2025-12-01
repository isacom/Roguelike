using System.IO;
using UnityEngine;

public static class SaveSystem
{
    private static string GetPath(string fileName)
    {
        return Path.Combine(Application.persistentDataPath, fileName);
    }

    public static void SaveLevelData(BoardManager.LevelData data, string fileName)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(GetPath(fileName), json);
        Debug.Log("Partida guardada en: " + GetPath(fileName));
    }

    public static BoardManager.LevelData LoadLevelData(string fileName)
    {
        string path = GetPath(fileName);

        if (!File.Exists(path))
        {
            Debug.LogWarning("No existe archivo de guardado: " + path);
            return null;
        }

        string json = File.ReadAllText(path);
        return JsonUtility.FromJson<BoardManager.LevelData>(json);
    }
}