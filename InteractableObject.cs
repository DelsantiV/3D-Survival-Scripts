using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    public string objectName;
    public bool isPickableItem;
    private string displayedName;

    private void Start()
    {
        if (isPickableItem) { displayedName = objectName + " [E to pick up]"; }
        else { displayedName = objectName; }
    }

    public string GetObjectName()
    {
        return objectName;
    }

    public string DisplayedObjectName() 
    { 
        return displayedName; 
    }
}
