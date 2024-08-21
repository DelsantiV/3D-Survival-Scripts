using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquippedItem : MonoBehaviour
{
    public ItemPile ItemPile { get; private set; }

    private void Awake()
    {
        
    }
    public void Remove()
    {
        Debug.Log("Removing item !");
        Destroy(gameObject);
    }

    public void Use()
    {

    }
}
