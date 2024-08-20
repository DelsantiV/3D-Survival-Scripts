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
            List<GeneralItem> itemsList = itemNamesList.ConvertAll<GeneralItem>(itemName => ItemManager.GetItemByName(itemName)); // Handle case when ItemManager.GetItemByName returns null
            itemPile = new ItemPile(itemsList);
            foreach (GeneralItem item in itemPile.ItemsInPile)
            {
                Debug.Log(item.ItemSO.name);
            }
        }
    }

    private void SpawnPile()
    {
        Debug.Log(itemPile.ToString());
        if (itemPile.ItemsInPile != null)
        {
            Debug.Log("Spawning Pile !");
            itemPile?.SpawnInWorld(transform.position);
        }
    }
}
