using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
{
    [SerializeField] private List<TKey> ids = new List<TKey>();
    [SerializeField] private List<TValue> values = new List<TValue>();

    //save the dictionary to lists
    public void OnBeforeSerialize()
    {
        ids.Clear();
        values.Clear();
        foreach (KeyValuePair<TKey, TValue> pair in this)
        {
            ids.Add(pair.Key);
            values.Add(pair.Value);
        }
    }

    //load dictionary from list
    public void OnAfterDeserialize()
    {
        this.Clear();
        for (int i = 0; i < ids.Count; i++)
        {
            this.Add(ids[i], values[i]);
        }
    }
}
