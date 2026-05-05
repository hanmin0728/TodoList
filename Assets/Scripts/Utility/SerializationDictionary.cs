using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public sealed class SerializationDictionary<TKey, TValue> : ISerializationCallbackReceiver
{
    [SerializeField] private List<TKey> keys = new List<TKey>();
    [SerializeField] private List<TValue> values = new List<TValue>();

    private readonly Dictionary<TKey, TValue> target = new Dictionary<TKey, TValue>();

    public int Count => target.Count;

    public TValue this[TKey key]
    {
        get => target.TryGetValue(key, out TValue value) ? value : default;
        set => target[key] = value;
    }

    public bool ContainsKey(TKey key)
    {
        return target.ContainsKey(key);
    }

    public bool TryGetValue(TKey key, out TValue value)
    {
        return target.TryGetValue(key, out value);
    }

    // 저장하기 직전에 실행: Dictionary -> List

    public void OnBeforeSerialize()
    {
        keys.Clear();
        values.Clear();

        foreach (KeyValuePair<TKey, TValue> pair in target)
        {
            keys.Add(pair.Key);
            values.Add(pair.Value);
        }
    }

    // 불러온 직후에 실행: List -> Dictionary
    public void OnAfterDeserialize()
    {
        target.Clear();

        int count = Math.Min(keys.Count, values.Count);
        for (int i = 0; i < count; i++)
        {
            target[keys[i]] = values[i];
        }
    }
}
