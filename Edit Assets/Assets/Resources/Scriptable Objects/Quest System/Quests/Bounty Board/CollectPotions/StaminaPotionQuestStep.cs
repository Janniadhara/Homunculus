using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaminaPotionQuestStep : QuestStep
{
    [SerializeField] private int idOfItemToCollect = 52;
    private int itemsCollected = 0;
    [SerializeField] private int itemsToComplete = 2;

    [SerializeField] private InventoryObject playerInventory;
    [SerializeField] private string journalText;

    private void OnEnable()
    {
        EventsManager.Instance.pickUpItemEvent.onItemPickUp += ItemCollected;
    }
    private void OnDisable()
    {
        EventsManager.Instance.pickUpItemEvent.onItemPickUp -= ItemCollected;
    }
    private void Start()
    {
        for (int i = 0; i < playerInventory.InventoryList.Count; i++)
        {
            ItemCollected(playerInventory.InventoryList[i].item, playerInventory.InventoryList[i].amount);
        }
        UpdateState();
    }
    private void ItemCollected(ItemObject item, int amount)
    {
        if (item.itemId == idOfItemToCollect)
        {
            if (itemsCollected < itemsToComplete)
            {
                itemsCollected += amount;
                UpdateState(); //brodcast everytime something changes
            }
        }
        if (itemsCollected >= itemsToComplete)
        {
            FinishQuestStep();
        }
    }
    private void UpdateState()
    {
        string state = itemsCollected.ToString();
        ChangeState(state, itemsToComplete.ToString(), journalText);
    }
    protected override void SetQuestStepState(string newState)
    {
        itemsCollected = System.Int32.Parse(newState);
        UpdateState();
    }
}
