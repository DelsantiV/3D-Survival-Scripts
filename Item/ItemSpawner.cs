using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{

    [SerializeField] private string itemName;
    // Start is called before the first frame update
    void Awake()
    {
        GeneralItem item = ItemManager.GetItemByName(itemName);
        if (item != null)
        {
            if (item.ItemPrefab != null)
            {
                GameObject itemPrefab = Instantiate(item.ItemPrefab, transform.position, transform.rotation);
                itemPrefab.AddComponent<ItemInWorld>();
                itemPrefab.AddComponent<Rigidbody>();
                itemPrefab.GetComponent<ItemInWorld>().item = item;
                Destroy(gameObject);
            }
        }
    }
}
