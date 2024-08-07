using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;

public class ItemInWorld : MonoBehaviour
{
    public GeneralItem item;
    public void PickUpItem(PlayerManager player)
    {
        if (item != null && player.HandsInventory != null)
        {
            bool isCollected = player.TryCollectItem(this);
            if (isCollected)
            {
                Destroy(gameObject);
                Debug.Log("Picked up " + ItemName + "!");
            }
            else
            {
                Debug.Log("Cannot collect " + ItemName + " !");
            }
        }
        else { Debug.Log("Problemos"); }
    }

    public GameObject ItemPrefab { get { return item.ItemSO.itemPrefab; } }
    public Sprite ItemSprite { get { return item.ItemSO.iconInInventory; } }
    public int StackSize { get { return item.ItemSO.maxStackSize; } }
    public string ItemName { get { return item.ItemSO.name; } }
    public string ObjectName { get { return item.ItemSO.name + " [E to pick up]"; } }


    private void Update()
    {
        if (transform.position.y < -50) { Destroy(gameObject); }
    }
}
