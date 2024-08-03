using System.Collections;
using System.Collections.Generic;
using TMPro;
using TMPro.EditorUtilities;
using UnityEngine;
using UnityEngine.UI;

public class CraftingUI : BasicUI
{

    private GameObject craftingPanelTemplate;
    private GameObject craftingButtonTemplate;

    [SerializeField] private CraftingPanel_SO[] craftingPanelsInfos;
    [SerializeField] private float craftingPanelXOffset;
    [SerializeField] private float craftingPanelYOffset;
    [SerializeField] private float craftingButtonXOffset;
    [SerializeField] private float craftingButtonYOffset;
    private Vector2 craftingPanelButtonInitialPosition;
    private Vector2 craftingPanelInitialPosition;
    private Vector2 craftingButtonInitialPosition;

    // Start is called before the first frame update
    void Awake()
    {
        craftingPanelTemplate.SetActive(false);
        craftingButtonTemplate.SetActive(false);
    }

    private void OpenCraftingPanel(BasicUI panel) 
    {
        foreach(Transform uiTransform in transform.root.Find("CraftingMain").GetComponentsInChildren<Transform>()) 
        { 
            if (uiTransform.gameObject.name == "CraftingPanelTemplate(Clone)" && uiTransform.Find("CraftingPanel").GetComponent<BasicUI>() != panel)
            {
                uiTransform.gameObject.SetActive(false); 
            }
        }
        panel.OpenUI(); 
    }

    private void CloseCraftingPanel(BasicUI panel) 
    { 
        panel.CloseUI();
        foreach (Transform uiTransform in transform.root.Find("CraftingMain").GetComponentsInChildren<Transform>(true)) 
        { 
            if (uiTransform.gameObject.name == "CraftingPanelTemplate(Clone)") { uiTransform.gameObject.SetActive(true);}
        }
    }

    private void GenerateCraftingPanelFromTemplate(CraftingManager craftingManager, CraftingPanel_SO panelInfos, float panelXOffset, float panelYOffset)
    {
        GameObject craftingPanel = Instantiate(craftingPanelTemplate, transform);
        craftingPanel.GetComponent<RectTransform>().anchoredPosition = craftingPanelButtonInitialPosition + new Vector2(panelXOffset, panelYOffset);
        craftingPanel.SetActive(true);
        craftingPanel.transform.Find("Title").GetComponent<TextMeshProUGUI>().SetText(panelInfos.title);
        craftingPanel.transform.Find("Image").GetComponent<Image>().sprite = panelInfos.panelIcon;

        GameObject panel = craftingPanel.transform.Find("CraftingPanel").gameObject;
        panel.transform.position = Vector3.zero;
        panel.GetComponent<RectTransform>().anchoredPosition = craftingPanelInitialPosition - new Vector2(panelXOffset, panelYOffset);
        panel.transform.Find("Title").GetComponent<TextMeshProUGUI>().SetText(panelInfos.title);
        BasicUI panelUI = panel.GetComponent<BasicUI>();
        CloseCraftingPanel(panelUI);
        craftingPanel.GetComponent<Button>().onClick.AddListener(delegate { OpenCraftingPanel( panelUI); });
        panel.transform.Find("CloseButton").GetComponent<Button>().onClick.AddListener(delegate { CloseCraftingPanel(panelUI); });

        int count = 0;
        foreach(CraftingRecipes_SO craftingRecipe in panelInfos.craftingRecipes)
        {
            GenerateCraftingButtonFromTemplate(craftingManager, panelUI, craftingRecipe, craftingButtonXOffset * (count % 3), craftingButtonYOffset * (count / 3));
            count++;
        }

    }

    private void GenerateCraftingButtonFromTemplate(CraftingManager craftingManager, BasicUI craftingPanel, CraftingRecipes_SO craftingRecipe, float craftingButtonXOffset, float craftingButtonYOffset)
    {
        GameObject craftingButtonGO = Instantiate(craftingButtonTemplate, craftingPanel.transform);
        craftingButtonGO.GetComponent<RectTransform>().anchoredPosition = craftingButtonInitialPosition + new Vector2(craftingButtonXOffset, craftingButtonYOffset);
        craftingButtonGO.SetActive(true);
        CraftingButton craftingButton = craftingButtonGO.GetComponent<CraftingButton>();
        craftingButton.SetCraftingButton(craftingManager, craftingRecipe);
    }

    public void GenerateCompleteUI(CraftingManager craftingManager)
    {
        craftingPanelTemplate = Resources.Load<GameObject>("UI/CraftingPanelTemplate");
        craftingButtonTemplate = Resources.Load<GameObject>("UI/CraftingButtonTemplate");
        craftingPanelButtonInitialPosition = craftingPanelTemplate.GetComponent<RectTransform>().anchoredPosition;
        craftingPanelInitialPosition = craftingPanelTemplate.transform.Find("CraftingPanel").GetComponent<RectTransform>().anchoredPosition;
        craftingButtonInitialPosition = craftingButtonTemplate.GetComponent<RectTransform>().anchoredPosition;
        int count = 0;
        foreach(CraftingPanel_SO panelInfos in craftingPanelsInfos) 
        {
            GenerateCraftingPanelFromTemplate(craftingManager, panelInfos, craftingPanelXOffset*(count % 3), craftingPanelYOffset*(count / 3));
            count++;
        }
    }

}
