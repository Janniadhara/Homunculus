using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NpcDialogue : MonoBehaviour
{
    public TextAsset npcJson;
    [SerializeField] private bool facePlayer;

    private bool isTalkinToPlayer;
    private Quaternion defaultRotation;

    private void OnEnable()
    {
        EventsManager.Instance.dialogueEvent.onStartDialogue += CheckTalkingToMe;
        EventsManager.Instance.dialogueEvent.onFinishDialogue += FinishTalking;
    }
    private void Start()
    {
        isTalkinToPlayer = false;
        defaultRotation = transform.rotation;
    }
    private void Update()
    {
        if (facePlayer && isTalkinToPlayer)
        {
            //rotate towards the player to face them
            Vector3 playerPos = GameObject.FindGameObjectWithTag("Player").transform.position;
            Vector3 npcPos = transform.position;
            Vector3 nTop = new Vector3(playerPos.x - npcPos.x, 0.0f, playerPos.z - npcPos.z);
            Quaternion rotation = Quaternion.LookRotation(nTop);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 4f);
        }
        else
        {
            //rotate back to rotation bevore the conversation started 
            //disabled cause npc movement needs to rotate
            //transform.rotation = Quaternion.Slerp(transform.rotation, defaultRotation, Time.deltaTime * 4f); ;
        }
    }
    private void CheckTalkingToMe(TextAsset npcJson, string npcName)
    {
        if (npcName == transform.name)
        {
            //isTalkinToPlayer = true;
        }
    }
    private void FinishTalking(string npcName)
    {
        if (npcName == transform.name)
        {
            //isTalkinToPlayer = false;
        }
    }
}
