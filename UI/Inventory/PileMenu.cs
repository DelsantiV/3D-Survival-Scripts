using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;

namespace GoTF.Content
{
    public class PileMenu : BasicUI
    {
        private ItemPileInInventory pileInInventory;

        private TextMeshProUGUI pileMenuTitle;
        private TextMeshProUGUI pileInfoText;

        private Button closeButton;
        private Button dropButton;
        private void Awake()
        {
            closeButton = transform.Find("CloseButton").GetComponent<Button>();
            closeButton.onClick.AddListener(CloseUI);

            dropButton = transform.Find("DropButton").GetComponent<Button>();

            pileMenuTitle = transform.Find("PileMenuTitle").GetComponent<TextMeshProUGUI>();
            pileInfoText = transform.Find("ItemInfoText").GetComponent<TextMeshProUGUI>();
        }

        public void SetPile(ItemPileInInventory pileInInventory)
        {
            this.pileInInventory = pileInInventory;
            FillMenuWithPileInfo(pileInInventory.ItemPile);
            dropButton.onClick.AddListener(pileInInventory.DropPile);
        }

        public void FillMenuWithPileInfo(ItemPile pile)
        {
            pileInfoText.text = pile.ToString();
        }
    }
}