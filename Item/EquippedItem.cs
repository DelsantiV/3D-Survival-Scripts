using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquippedItem : MonoBehaviour
{
    public ItemPile ItemPile { get; private set; }
    public ItemPileInWorld itemsInWorld { get; private set; }

    private void Awake()
    {
        itemsInWorld = GetComponent<ItemPileInWorld>();
    }
    public void Remove()
    {
        Debug.Log("Removing item !");
        Destroy(gameObject);
    }

    public void Drop()
    {
        transform.parent = null;
        itemsInWorld.FromHandToGround();
    }

    public void Use()
    {

    }
}
