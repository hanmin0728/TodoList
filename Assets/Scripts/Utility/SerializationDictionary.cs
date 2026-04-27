using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.Rendering.DebugUI;

[Serializable]
public class SerializationDictionary<TKey, TValue> : ISerializationCallbackReceiver
{
    [SerializeField] private List<TKey> keys = new List<TKey>();
    [SerializeField] private List<TValue> values = new List<TValue>();

    // 실제 로직에서 사용할 딕셔너리 (직렬화는 안 됨)
    private Dictionary<TKey, TValue> target = new Dictionary<TKey, TValue>();

    public Dictionary<TKey, TValue> ToDictionary() => target;

    // 딕셔너리처럼 쓰기 편하게 인덱서 구현
    public TValue this[TKey key]
    {
        get => target.ContainsKey(key) ? target[key] : default;
        set => target[key] = value;
    }

    public bool ContainsKey(TKey key) => target.ContainsKey(key);

    // 저장하기 직전에 실행: Dictionary -> List
    public void OnBeforeSerialize()
    {
        keys.Clear();
        values.Clear();
        foreach (var pair in target)
        {
            keys.Add(pair.Key);
            values.Add(pair.Value);
        }
    }

    // 불러온 직후에 실행: List -> Dictionary
    public void OnAfterDeserialize()
    {
        target.Clear();
        for (int i = 0; i < Math.Min(keys.Count, values.Count); i++)
        {
            target[keys[i]] = values[i];
        }
    }
}