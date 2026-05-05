using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public sealed class CSVManager : Singleton<CSVManager>
{
    [Header("CSV file names in Resources")]
    [FormerlySerializedAs("_fileNames")]
    [SerializeField] private List<string> fileNames = new List<string>();

    private readonly Dictionary<string, List<Dictionary<string, object>>> tablesByName = new Dictionary<string, List<Dictionary<string, object>>>();

    public bool IsInitialized { get; private set; }
    public event Action OnLoadingComplete;

    protected override void Awake()
    {
        base.Awake();
        if (Instance != this)
        {
            return;
        }

        LoadAllTables();
    }

    public List<Dictionary<string, object>> GetTable(string fileName)
    {
        if (tablesByName.TryGetValue(fileName, out List<Dictionary<string, object>> table))
        {
            return table;
        }

        Debug.LogError($"[CSVManager] Table is missing. Name: {fileName}");
        return null;
    }

    public Dictionary<string, object> GetDataById(string fileName, string idColumnName, string idValue)
    {
        List<Dictionary<string, object>> table = GetTable(fileName);
        if (table == null)
        {
            return null;
        }

        for (int i = 0; i < table.Count; i++)
        {
            Dictionary<string, object> row = table[i];
            if (row.TryGetValue(idColumnName, out object value) && value.ToString() == idValue)
            {
                return row;
            }
        }

        return null;
    }

    private void LoadAllTables()
    {
        tablesByName.Clear();

        for (int i = 0; i < fileNames.Count; i++)
        {
            string fileName = fileNames[i];
            if (string.IsNullOrEmpty(fileName))
            {
                continue;
            }

            List<Dictionary<string, object>> data = CSVReader.Read(fileName);
            if (data != null && !tablesByName.ContainsKey(fileName))
            {
                tablesByName.Add(fileName, data);
            }
        }

        IsInitialized = true;
        OnLoadingComplete?.Invoke();
        Debug.Log("[CSVManager] CSV tables loaded.");
    }
}
