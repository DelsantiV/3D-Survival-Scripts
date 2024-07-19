using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;

public class Item : MonoBehaviour
{
    public Item_General_SO itemSO;
    private string displayName;

    // Start is called before the first frame update
    void Start()
    {
        
        displayName = itemSO.itemName + " [E to pick up]";
    }


    public void PickUpItem(PlayerManager player)
    {
        if ( itemSO != null && player.inventory != null)
        {
            player.inventory.AddItemToInventory(itemSO, 3);
            Destroy(gameObject);
            Debug.Log("Picked up " + displayName + "!");
        }
        else { Debug.Log("Problemos"); }
    }

    public GameObject GetItemPrefab() { return itemSO.itemPrefab; }
    public Sprite GetItemSprite() { return itemSO.iconInInventory; }
    public int GetStackSize() { return itemSO.maxStackSize; }
    public string GetItemName() {  return itemSO.name; }
    public string DisplayObjectName() { return displayName; }


    private void Update()
    {
        if (transform.position.y < -50) { Destroy(gameObject); }
    }
}
