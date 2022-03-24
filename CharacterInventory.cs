using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterInventory : MonoBehaviour
{
    private bool isActive = false;
    public GameObject InteratctioMenu;
    public GameObject[] equippedItemSlots;
    private GameObject[] equippedItems;
    private string[] itemName;
    public int numItems = 4;
    private bool[] isFull;
    public GameObject EquippedItem;

   
    // Start is called before the first frame update
    void Start()
    {
        //Gotta make it to where the inventory can be scalable.
        itemName = new string[numItems];
        isFull = new bool[numItems];
        equippedItems = new GameObject[numItems];

        for(int i = 0; i < numItems; i++)
        {
            itemName[i] = "None";
        }

        //TempCode
        itemName[0] = "Glock19";

        //Change the alpha
        for (int i = 0; i < numItems; i++)
        {
            GenerateItem(i, itemName[i]);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I) && isActive == false)
        {
            OpenMenu();
        }
        else if (Input.GetKeyDown(KeyCode.I) && isActive == true)
        {
            CloseMenu();
        }
    }

    private void CloseMenu()
    {
        InteratctioMenu.SetActive(false);
        isActive = false;
    }

    private void OpenMenu()
    {
        InteratctioMenu.SetActive(true);
        isActive = true;
    }

    public GameObject EquipItem(int num)
    {
        num -= 1;
        if (num < 0 || num >= numItems)
        {
            EquippedItem.GetComponent<Image>().sprite = Resources.Load<Sprite>("Textures/ItemEquippedIcons/" + itemName[0]);
            return equippedItems[0];
        }
        else
        {
            EquippedItem.GetComponent<Image>().sprite = Resources.Load<Sprite>("Textures/ItemEquippedIcons/" + itemName[num]);
            Color tempColor = EquippedItem.GetComponent<Image>().color;
            tempColor.a = 1f;
            EquippedItem.GetComponent<Image>().color = tempColor;
            return equippedItems[num];
        }
    }

    private void GenerateItem(int itemNumInArray, string itemName)
    {
        if (itemName != "None")
        {
            GameObject NewItem = Instantiate(Resources.Load("Prefabs/Weapons/" + itemName) as GameObject);
            NewItem.SetActive(false);
            isFull[itemNumInArray] = true;
            equippedItems[itemNumInArray] = NewItem;
            equippedItemSlots[itemNumInArray].GetComponent<Image>().sprite = Resources.Load<Sprite>("Textures/ItemEquippedIcons/" + itemName);
            Color tempColor = equippedItemSlots[itemNumInArray].GetComponent<Image>().color;
            tempColor.a = 1f;
            equippedItemSlots[itemNumInArray].GetComponent<Image>().color = tempColor;
        }
        else
        {
            isFull[itemNumInArray] = false;
        }
        
    }
}
