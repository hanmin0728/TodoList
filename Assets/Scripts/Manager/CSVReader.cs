using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public static class CSVReader 
{
    // CSV의 쉼표와 따옴표 등을 구분하기 위한 정규표현식
    static string SPLIT_RE = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";
    static string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";

    public static List<Dictionary<string, object>> Read(string file)
    {
        var list = new List<Dictionary<string, object>>();

        // Resources 폴더에서 파일을 읽어옴 (확장자 제외)
        TextAsset data = Resources.Load(file) as TextAsset;

        if (data == null)
        {
            Debug.LogError($"[CSVReader] 파일을 찾을 수 없습니다: Resources/{file}");
            return null;
        }

        var lines = Regex.Split(data.text, LINE_SPLIT_RE);
        if (lines.Length <= 1) return list;

        // 첫 번째 줄(Header) 추출
        var header = Regex.Split(lines[0], SPLIT_RE);

        for (var i = 1; i < lines.Length; i++)
        {
            var values = Regex.Split(lines[i], SPLIT_RE);
            if (values.Length == 0 || values[0] == "") continue;

            var entry = new Dictionary<string, object>();
            for (var j = 0; j < header.Length && j < values.Length; j++)
            {
                string value = values[j];
                value = value.TrimStart('\"').TrimEnd('\"').Replace("\\", "");

                // 데이터 저장 (키: 컬럼명, 값: 데이터)
                entry[header[j]] = value;
            }
            list.Add(entry);
        }
        return list;
    }
}
