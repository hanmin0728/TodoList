using System;
using System.Collections.Generic;
using UnityEngine;

public class CSVManager : Singleton<CSVManager>
{
    [Header("로드할 CSV 파일 이름 (Resources 폴더 내)")]
    [SerializeField]
    private List<string> _fileNames = new List<string> { };

    // 데이터를 캐싱할 공간
    private Dictionary<string, List<Dictionary<string, object>>> _tables = new Dictionary<string, List<Dictionary<string, object>>>();

    public bool IsInitialized { get; private set; } = false;

    // 로딩이 완료되면 등록된 함수들에게 신호를 보냄
    public event Action OnLoadingComplete;
    protected override void Awake()
    {
        base.Awake(); // 싱글톤 초기화
        LoadAllTables();
    }

    private void LoadAllTables()
    {
        foreach (string name in _fileNames)
        {
            if (string.IsNullOrEmpty(name)) continue;
            List<Dictionary<string, object>> data = CSVReader.Read(name);
            if (data != null)
            {
                if (!_tables.ContainsKey(name)) _tables.Add(name, data);
            }
        }

        IsInitialized = true;

        // 로딩이 끝났음을 알림 (null 체크 포함)
        OnLoadingComplete?.Invoke();

        Debug.Log("<color=green>[CSVManager]</color> 모든 테이블 로드 및 이벤트 전파 완료.");
    }
  
    public List<Dictionary<string, object>> GetTable(string fileName)
    {
        if (_tables.TryGetValue(fileName, out var table))
            return table;

        Debug.LogError($"[CSVManager] {fileName} 테이블을 찾을 수 없습니다.");
        return null;
    }

    public Dictionary<string, object> GetDataById(string fileName, string idColumnName, string idValue)
    {
        var table = GetTable(fileName);
        if (table == null) return null;

        return table.Find(row => row[idColumnName].ToString() == idValue);
    }
}
