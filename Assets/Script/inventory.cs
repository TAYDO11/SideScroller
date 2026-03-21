using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.VisualScripting;

public class inventory : MonoBehaviour
{
    
    public static inventory instance;

    
    public Item[] inventories = new Item[4];
    public Transform inventoryDisplay;

    private Sprite blankItem;

    private void Awake()
    {
        instance = this;
    }


    private void Start()
    {
        blankItem = inventoryDisplay.transform.GetChild(0).transform.Find("icon").GetComponent<Image>().sprite;
    }

    public void LoadInventory()
    {
        for (int i = 0; i < inventories.Length; i++)
        {
            if (inventories[i] == null)
            {
                inventoryDisplay.transform.GetChild(i).transform.Find("icon").GetComponent<Image>().sprite = blankItem;

                continue;
            }

            inventoryDisplay.transform.GetChild(i).transform.Find("icon").GetComponent<Image>().sprite = inventories[i].icon;
        }
    }


}
