using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;

namespace GoTF.Content
{
    public class PileMenu : BasicUI
    {
        private ItemPile pile;

        private Button closeButton;
        private Button dropButton;
        private void Awake()
        {
            closeButton = transform.Find("CloseButton").GetComponent<Button>();
            closeButton.onClick.AddListener(CloseUI);
        }
    }
}