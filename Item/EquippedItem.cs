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
        Destroy(gameObject);
    }

    public void Drop()
    {
        transform.parent = null;
        itemsInWorld.AddRigidBodyToItems();
    }

    public void ChangeParent(Transform transform, bool worldPositionStays = false)
    {
        if (transform.parent == null)
        {
            Drop();
            return;
        }

        transform.SetParent(transform, worldPositionStays);
    }

    public void Use()
    {

    }
}
