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
        Debug.Log(item.StackSize);
        GameObject itemPrefab = Instantiate(item.ItemPrefab, transform.position, transform.rotation);
        itemPrefab.AddComponent<Item>();
        itemPrefab.AddComponent<Rigidbody>();
        itemPrefab.GetComponent<Item>().item = item;
        Destroy(gameObject);
    }
}
