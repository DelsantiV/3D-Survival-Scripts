using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandsInventory
{
    private PlayerManager player;
    private QuickSlot leftHandQuickSlot;
    private QuickSlot rightHandQuickSlot;
    private QuickSlot bothHandQuickSlot;
    private HandsManager handsManager;
    private ItemPile rightHandPile;
    private ItemPile leftHandPile;
    private ItemPile bothHandPile;
    public HandsInventory(PlayerManager player)
    {
        this.player = player;
        leftHandQuickSlot = player.LeftHandQuickSlot;
        rightHandQuickSlot = player.RightHandQuickSlot;
        bothHandQuickSlot = player.BothHandQuickSlot;
        handsManager = player.HandsManager;
    }

    public bool TryAddItemToHands(GeneralItem item)
    {
        return handsManager.TryEquipItemToNextEmptyHand(item);
    }
}
