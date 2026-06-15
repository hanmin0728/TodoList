using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public static class CSVReader 
{
    public static List<Dictionary<string, object>> Read(string file)
    {
        var list = new List<Dictionary<string, object>>();
        TextAsset data = Resources.Load<TextAsset>(file);

        if (data == null)
        {
            Debug.LogError($"[CSVReader] 파일을 찾을 수 없습니다: Resources/{file}");
            return null;
        }

        string[] lines = data.text.Split(new[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
        if (lines.Length <= 1) return list;

        string[] header = SplitCsvLine(lines[0]);

        for (int i = 1; i < lines.Length; i++)
        {
            string[] values = SplitCsvLine(lines[i]);
            if (values.Length == 0 || string.IsNullOrEmpty(values[0])) continue;

            var entry = new Dictionary<string, object>();
            for (int j = 0; j < header.Length && j < values.Length; j++)
            {
                entry[header[j]] = values[j];
            }
            list.Add(entry);
        }
        return list;
    }

    private static string[] SplitCsvLine(string line)
    {
        List<string> result = new List<string>();
        bool inQuotes = false;
        int start = 0;

        for (int i = 0; i < line.Length; i++)
        {
            if (line[i] == '\"')
            {
                inQuotes = !inQuotes; 
            }
            else if (line[i] == ',' && !inQuotes)
            {
                result.Add(CleanValue(line.Substring(start, i - start)));
                start = i + 1;
            }
        }
        result.Add(CleanValue(line.Substring(start)));
        return result.ToArray();
    }

    private static string CleanValue(string value)
    {
        value = value.Trim();
        if (value.StartsWith("\"") && value.EndsWith("\""))
        {
            value = value.Substring(1, value.Length - 2);
        }
        return value.Replace("\"\"", "\"").Replace("\\", "");
    }
}
