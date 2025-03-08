using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoTF.Content
{
    public class BasicUI : MonoBehaviour
    {
        public virtual RectTransform RectTransform { get { return gameObject.GetComponent<RectTransform>(); } }
        public virtual void OpenUI() { gameObject.SetActive(true); }
        public virtual void CloseUI() { gameObject.SetActive(false); }
        public virtual void SetActive(bool active) { if (active) OpenUI(); else CloseUI(); }
        public bool IsOpen() { return gameObject.activeInHierarchy; }

        public virtual void SetPosition(Vector2 position) { RectTransform.anchoredPosition = position; }
    }
}
