using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{

    [SerializeField] private List<string> itemNamesList;

    private struct ItemWithAmount
    {
        public string itemName;
        public int amount;
    }
    private ItemPile itemPile; 
    // Start is called before the first frame update
    void Awake()
    {
        InitializeSpawner();
        SpawnPile();
    }

    private void InitializeSpawner()
    {
        if (itemNamesList.Count > 0)
        {
            List<GeneralItem> itemsList = itemNamesList.ConvertAll(itemName => ItemManager.GetItemByName(itemName)); // Handle case when ItemManager.GetItemByName returns null
            itemPile = new ItemPile(itemsList);
        }
    }

    private void SpawnPile()
    {
        if (itemPile.ItemsInPile != null)
        {
            Debug.Log("Spawning " + itemPile.ToString());
            itemPile.SpawnInWorld(transform.position);
        }
    }
}
