using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;

public class Item : MonoBehaviour
{
    public GeneralItem item;
    private string displayName;

    // Start is called before the first frame update
    void Start()
    {
        
        displayName = item.ItemSO.name + " [E to pick up]";
    }


    public void PickUpItem(PlayerManager player)
    {
        if (item != null && player.inventory != null)
        {
            player.inventory.AddItemToInventory(item, 3);
            Destroy(gameObject);
            Debug.Log("Picked up " + displayName + "!");
        }
        else { Debug.Log("Problemos"); }
    }
    public void PickUpItem(PlayerManagerV2 player)
    {
        if (item != null && player.inventory != null)
        {
            player.inventory.AddItemToInventory(item, 3);
            Destroy(gameObject);
            Debug.Log("Picked up " + displayName + "!");
        }
        else { Debug.Log("Problemos"); }
    }

    public GameObject ItemPrefab { get { return item.ItemSO.itemPrefab; } }
    public Sprite ItemSprite { get { return item.ItemSO.iconInInventory; } }
    public int StackSize { get { return item.ItemSO.maxStackSize; } }
    public string ItemName { get { return item.ItemSO.name; } }
    public string DisplayObjectName() { return displayName; }


    private void Update()
    {
        if (transform.position.y < -50) { Destroy(gameObject); }
    }
}
