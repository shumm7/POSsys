using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GoodsController : MonoBehaviour
{
    public int ButtonMode;
    public Button Tab1;
    public Button Tab2;
    public Button Tab3;
    public Button Tab4;
    public Button TabName;
    public GameObject GoodsUI;
    public GameObject TabNameUI;

    void Start()
    {
        ButtonMode = 0;
        ButtonPressed(ButtonMode);
    }

    public void ButtonPressed(int num)
    {
        ButtonMode = num;
        if(num>=0 && num <= 3)
        {
            GoodsUI.SetActive(true);
            TabNameUI.SetActive(false);
            setGoodsData();
        }
        else if (num==4)
        {
            GoodsUI.SetActive(false);
            TabNameUI.SetActive(true);

            DataLoader.List tempList = GetComponent<DataLoader>().LoadList();
            for (int i = 0; i < 4; i++)
            {
                TabNameUI.transform.Find("Tab" + (i + 1).ToString()).GetComponent<InputField>().text = tempList.CategoryName[i];
            }
        }
    }

    public void setGoodsData()
    {
        DataLoader.List tempList = GetComponent<DataLoader>().LoadList();
        for (int i = 0; i < 6; i++) {
            Transform temp = GoodsUI.transform.Find((i+1).ToString());
            temp.Find("Name").GetComponent<InputField>().text = tempList.ProductName[ButtonMode][i];
            temp.Find("Price").GetComponent<InputField>().text = tempList.Price[ButtonMode][i].ToString();
            temp.Find("Stock").GetComponent<InputField>().text = tempList.Stock[ButtonMode][i].ToString();
            temp.Find("Available").GetComponent<Toggle>().isOn = tempList.Available[ButtonMode][i];
        }
    }

    public void saveGoodsData()
    {
        DataLoader.List tempList = GetComponent<DataLoader>().LoadList();
        for (int i = 0; i < 6; i++)
        {
            Transform temp = GoodsUI.transform.Find((i+1).ToString());
            tempList.ProductName[ButtonMode][i] = temp.Find("Name").GetComponent<InputField>().text;
            if (temp.Find("Price").GetComponent<InputField>().text != "")
            {
                tempList.Price[ButtonMode][i] = int.Parse(temp.Find("Price").GetComponent<InputField>().text);
            }
            else
            {
                tempList.Price[ButtonMode][i] = 0;
            }
            if (temp.Find("Stock").GetComponent<InputField>().text != "")
            {
                tempList.Stock[ButtonMode][i] = int.Parse(temp.Find("Stock").GetComponent<InputField>().text);
            }
            else
            {
                tempList.Stock[ButtonMode][i] = 0;
            }
            tempList.Available[ButtonMode][i] = temp.Find("Available").GetComponent<Toggle>().isOn;
        }
        GetComponent<DataLoader>().SaveList(tempList);
    }

    public void saveTabName()
    {
        DataLoader.List tempList = GetComponent<DataLoader>().LoadList();
        for (int i = 0; i < 4; i++)
        {
            tempList.CategoryName[i] = TabNameUI.transform.Find("Tab" + (i + 1).ToString()).GetComponent<InputField>().text;
        }
        GetComponent<DataLoader>().SaveList(tempList);
    }
}
