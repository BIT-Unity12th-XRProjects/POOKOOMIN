using System.IO;
using UnityEngine;

public class DataLoader<TModel, TRaw> 
    where TModel : IDataConvertible<TRaw>, new() 
    where TRaw : class
{
    private readonly string filePath;

    public DataLoader(string fileName)
    {
        filePath = Path.Combine(Application.persistentDataPath, fileName);
    }

    public void Save(TModel model)
    {
        TRaw raw = model.ToRaw();
        string json = JsonUtility.ToJson(raw, true);
        File.WriteAllText(filePath, json);
        Debug.Log($"Saved to {filePath}");
    }

    public TModel Load()
    {
        if (!File.Exists(filePath))
        {
            Debug.LogWarning($"File not found at {filePath}. Returning default.");
            return new TModel(); 
        }

        string json = File.ReadAllText(filePath);
        TRaw raw = JsonUtility.FromJson<TRaw>(json);

        TModel model = new TModel();
        model.FromRaw(raw);
        return model;
    }
}
