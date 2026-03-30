using System;
using System.Collections.Generic;

using UnityEngine;

// Unity 官方提供的 SerializableDictionary 基础类
[Serializable]
public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
{
    [SerializeField]
    private List<TKey> keys = new List<TKey>();

    [SerializeField]
    private List<TValue> values = new List<TValue>();

    // 序列化前：将字典转换为两个列表
    public void OnBeforeSerialize()
    {
        keys.Clear();
        values.Clear();

        foreach (KeyValuePair<TKey, TValue> pair in this)
        {
            keys.Add(pair.Key);
            values.Add(pair.Value);
        }
    }

    // 反序列化后：将列表转换回字典
    public void OnAfterDeserialize()
    {
        this.Clear();

        if (keys.Count != values.Count)
        {
            Debug.LogError($"键值数量不匹配！键数: {keys.Count}, 值数: {values.Count}");
            return;
        }

        for (int i = 0; i < keys.Count; i++)
        {
            this.Add(keys[i], values[i]);
        }
    }
}

// 具体使用示例
[Serializable]
public class StringIntDictionary : SerializableDictionary<string, int> { }

[Serializable]
public class GameObjectFloatDictionary : SerializableDictionary<GameObject, float> { }

[Serializable] 
public class IntBoolDictionary : SerializableDictionary<int, bool> { }
[ Serializable]
public class ItemIdCountDictionary : SerializableDictionary<int, int> { }


