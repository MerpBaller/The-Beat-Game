using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractionMenuCreator : MonoBehaviour
{
    public GameObject InteractionMenu;
    public GameObject BottomBar;
    public GameObject Button;
    private float menuStartY = -785.5f;
    private float buttonStartY = -482.5f;
    private float menuIncreaseY = 78.5f;
    private float buttonIncreaseY = 72.5f;
    private GameObject temp;
    public int numOptions = 1;
    private int prevNumOption;
    private int maxOptions = 8;
    private GameObject menu;
    private GameObject bottom;
    private bool menuFlag = false;
    public List<string> buttonTexts;
    void Awake()
    {
        gameObject.SetActive(true);
        
        if (prevNumOption != numOptions)
        {
            prevNumOption = numOptions;
            menuFlag = false;
        }
        
        if (menuFlag == false)
        {
            if (numOptions == 1)
            {
                menu = Instantiate(InteractionMenu, gameObject.transform);
                bottom = Instantiate(BottomBar, gameObject.transform);
                temp = Instantiate(Button, gameObject.transform);
                temp.transform.localPosition = new Vector2(temp.transform.localPosition.x, buttonStartY + (buttonIncreaseY * 0));
                temp.GetComponentInChildren<Text>().text = buttonTexts[0];

                menu.SetActive(true);
                bottom.SetActive(true);
                temp.SetActive(true);

                menuFlag = true;
            }
            else if (numOptions > 1 && numOptions <= maxOptions)
            {
                menu = Instantiate(InteractionMenu, gameObject.transform);
                bottom = Instantiate(BottomBar, gameObject.transform);

                menu.transform.localPosition = new Vector2(menu.transform.localPosition.x, menuStartY + (menuIncreaseY * (numOptions - 1)));

                for (int i = 0; i < numOptions; i++)
                {
                    temp = Instantiate(Button, gameObject.transform);
                    temp.transform.localPosition = new Vector2(temp.transform.localPosition.x, buttonStartY + (buttonIncreaseY * i));
                    temp.GetComponentInChildren<Text>().text = buttonTexts[i];
                    temp.SetActive(true);
                }

                menu.SetActive(true);
                bottom.SetActive(true);

                menuFlag = true;
            }
        }
    }
}
