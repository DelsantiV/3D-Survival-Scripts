using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicUI : MonoBehaviour
{
    public virtual void OpenUI() { gameObject.SetActive(true); }
    public virtual void CloseUI() { gameObject.SetActive(false); }
    public bool IsOpen() { return gameObject.activeInHierarchy; }
}
