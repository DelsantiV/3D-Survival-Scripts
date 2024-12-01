using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoTF.Content
{
    public class BasicUI : MonoBehaviour
    {
        public virtual void OpenUI() { gameObject.SetActive(true); }
        public virtual void CloseUI() { gameObject.SetActive(false); }
        public virtual void SetActive(bool active) { gameObject.SetActive(active); }
        public bool IsOpen() { return gameObject.activeInHierarchy; }
    }
}
