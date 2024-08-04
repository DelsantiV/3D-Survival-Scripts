using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquippedItem : MonoBehaviour
{
    public GeneralItem Item { get; private set; }

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
