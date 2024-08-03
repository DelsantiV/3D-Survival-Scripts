using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class GeneralInventoryUI : BasicUI
{
    private GameObject slotTemplate;
    private GameObject inventoryBackgroundTemplate;
    public int numberOfSlotsX;

    public GameObject slotGroup;

    [SerializeField] private float slotXOffset;
    [SerializeField] private float slotYOffset;

    private Vector2 firstSlotPosition = new Vector2(-166,64);
    private Vector2 inventoryCenterPosition = Vector2.zero;


    public virtual void CreateInventoryUI(int numberOfSlots)
    {
        slotTemplate = Resources.Load<GameObject>("UI/SlotTemplate");
        // Could implement a scale factor to change slot size
        Vector2 slotSize = slotTemplate.GetComponent<RectTransform>().sizeDelta;

        inventoryBackgroundTemplate = Resources.Load<GameObject>("UI/InventoryBackground");
        // Background could depend on the invenotry type (e.g player != chest)

        GameObject inventoryBackground = Instantiate(inventoryBackgroundTemplate, transform);
        inventoryBackground.GetComponent<RectTransform>().anchoredPosition = inventoryCenterPosition;
        inventoryBackground.GetComponent<RectTransform>().sizeDelta = new Vector2((slotXOffset * (float)(numberOfSlotsX + 0.5)), -slotYOffset * (float)(numberOfSlots / numberOfSlotsX + 0.5));
        slotGroup = new GameObject("Slots");
        slotGroup.transform.SetParent(transform, false);
        slotGroup.transform.position = Vector3.zero;
        slotGroup.AddComponent<RectTransform>();
        slotGroup.GetComponent<RectTransform>().anchoredPosition = inventoryCenterPosition;

        for (int i = 0; i < numberOfSlots; i++)
        {
            GameObject slot = Instantiate(slotTemplate, slotGroup.transform);
            slot.GetComponent<RectTransform>().anchoredPosition = firstSlotPosition + new Vector2(slotXOffset * (i % numberOfSlotsX), slotYOffset * (i / numberOfSlotsX));
        }
    }

}
