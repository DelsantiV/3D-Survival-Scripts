using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquippedItem : MonoBehaviour
{
    public void Remove()
    {
        Debug.Log("Removing item !");
        Destroy(gameObject);
    }
}
