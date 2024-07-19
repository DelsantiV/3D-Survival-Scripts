using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{

    [SerializeField] private Item_General_SO itemSO;
    // Start is called before the first frame update
    void Awake()
    {
        GameObject itemPrefab = Instantiate(itemSO.itemPrefab, transform.position, transform.rotation);
        itemPrefab.AddComponent<Item>();
        itemPrefab.AddComponent<Rigidbody>();
        itemPrefab.GetComponent<Item>().itemSO = itemSO;
        Destroy(gameObject);
    }
}
