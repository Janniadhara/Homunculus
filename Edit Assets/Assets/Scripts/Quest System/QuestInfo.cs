using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "QuestInfo", menuName = "QuestSystem/QuestInfo")]
public class QuestInfo : ScriptableObject
{
    [field: SerializeField] public string id { get; private set; }

    [Header("General")]
    public string displayName;
    [TextArea] public string description;
    public string questGiver;

    [Header("Requirements")]
    public int levelRequired;
    public QuestInfo[] questPrerequisites;

    [Header("Steps")]
    public GameObject[] questStepPrefabs;
    [Header("QuestStep Names")]
    public string[] questStepNames;

    [Header("Rewards")]
    public int xpReward;
    public int bronzeReward;
    public int silverReward;
    public int goldReward;

    private void OnValidate()
    {
#if UNITY_EDITOR
        id = this.name;
        UnityEditor.EditorUtility.SetDirty(this);
#endif
    }
}
