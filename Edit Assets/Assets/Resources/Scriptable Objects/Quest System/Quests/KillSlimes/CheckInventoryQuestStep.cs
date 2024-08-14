using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckInventoryQuestStep : QuestStep
{
    [SerializeField] private string npcName;
    [SerializeField] private InventoryObject playerInventory;
    [SerializeField] private ItemObject desiredItem;
    [SerializeField] private int amountNeeded;
    private bool canFinishQuestStep;

    private void OnEnable()
    {
        //check player inventory on change
        EventsManager.Instance.pickUpItemEvent.onPlayerInvChanged += CheckInventory;
        EventsManager.Instance.pickUpItemEvent.onTakeItemFromPlayer += TakeItems;
    }
    private void OnDisable()
    {
        EventsManager.Instance.pickUpItemEvent.onPlayerInvChanged -= CheckInventory;
        EventsManager.Instance.pickUpItemEvent.onTakeItemFromPlayer += TakeItems;
    }
    private void TakeItems(string questId)
    {
        if (questId == this.questId)
        {
            playerInventory.RemoveItemAmount(desiredItem, amountNeeded);
            FinishQuestStep();
        }
    }
    private void CheckInventory()
    {
        canFinishQuestStep = false;
        for (int i = 0; i < playerInventory.InventoryList.Count; i++)
        {
            if (playerInventory.InventoryList[i].item == desiredItem)
            {
                if (playerInventory.InventoryList[i].amount >= amountNeeded)
                {
                    canFinishQuestStep = true;
                    break;
                }
            }
        }
        if (canFinishQuestStep)
        {
            DialogueManager.Instance.SetVariableState("Kill5Slimes", "canFinish");
        }
        else
        {
            DialogueManager.Instance.SetVariableState("Kill5Slimes", "inProgress");
        }
    }
    private void UpdateState()
    {
        string state = amountNeeded.ToString();
        ChangeState(state, amountNeeded.ToString(), "");
    }
    protected override void SetQuestStepState(string newState)
    {
        amountNeeded = System.Int32.Parse(newState);
        UpdateState();
    }
}
