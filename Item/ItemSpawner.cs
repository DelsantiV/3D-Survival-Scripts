using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoTF.Content
{
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
                List<GeneralItem> itemsList = itemNamesList.ConvertAll(itemName => ItemManager.GetItemByName(itemName));
                itemsList.RemoveAll(item => item == null);
                itemPile = new ItemPile(itemsList);
            }
        }

        private void SpawnPile()
        {
            if (itemPile != null)
            {
                if (itemPile.NumberOfItemsInPile > 0)
                {
                    itemPile.SpawnInWorld(transform.position);
                }
            }
        }
    }
}
