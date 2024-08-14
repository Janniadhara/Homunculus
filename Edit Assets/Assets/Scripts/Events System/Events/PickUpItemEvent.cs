using System;

public class PickUpItemEvent
{
    public event Action<ItemObject, int> onItemPickUp;
    public void ItemPickUp(ItemObject item, int amount)
    {
        if (onItemPickUp != null)
        {
            onItemPickUp(item, amount);
        }
    }
    public event Action<AttachedItem> onHarvestPlant;
    public void HarvestPlant(AttachedItem item)
    {
        if (onHarvestPlant != null)
        {
            onHarvestPlant(item);
        }
    }
    public event Action<AttachedItem> onPlantHarvested;
    public void PlantHarvested(AttachedItem item)
    {
        if (onPlantHarvested != null)
        {
            onPlantHarvested(item);
        }
    }
    public event Action<AttachedItem> onMineMineral;
    public void MineMineral(AttachedItem item)
    {
        if (onMineMineral != null)
        {
            onMineMineral(item);
        }
    }
    public event Action<AttachedItem> onMineralMined;
    public void MineralMined(AttachedItem item)
    {
        if (onMineralMined != null)
        {
            onMineralMined(item);
        }
    }
    public event Action onPlayerInvChanged;
    public void PlayerInvChanged()
    {
        if (onPlayerInvChanged != null)
        {
            onPlayerInvChanged();
        }
    }
    public event Action<string> onTakeItemFromPlayer;
    public void TakeItemFromPlayer(string questId)
    {
        if (onTakeItemFromPlayer != null)
        {
            onTakeItemFromPlayer(questId);
        }
    }

}
