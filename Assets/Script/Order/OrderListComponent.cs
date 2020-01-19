using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class OrderListComponent : MonoBehaviour
{
    public GameObject Parent;
    public GameObject UpButton;
    public GameObject DownButton;
    public static int UpperNumber;
    
    void Start()
    {
        UpperNumber = 0;
    }

    public void UpdateList(GameManager.OrderList _list)
    {
        for (int i = 0; i < 15; i++)
        {
            Parent.transform.Find((i + 1).ToString()).gameObject.SetActive(false);
        }

        if (14 >= MaximumType(_list) - UpperNumber && UpperNumber > 0 && MaximumType(_list) > 15)
        {
            UpperNumber = MaximumType(_list) - 16;
        }

        if (MaximumType(_list) <= 15)
        {
            for (int i = 0; i < MaximumType(_list); i++)
            {
                GameObject temp = Parent.transform.Find((i + 1).ToString()).gameObject;
                temp.SetActive(true);
                int TabMode = _list.TabMode[i];
                int Number = _list.Number[i];
                temp.transform.Find("ProductName").GetComponent<Text>().text = GameManager.productList.ProductName[TabMode][Number-1];
                temp.transform.Find("Amount").GetComponent<Text>().text = "数量 " + _list.Amount[i].ToString();
                temp.transform.Find("Price").GetComponent<Text>().text = "￥" + GetComponent<GameManager>().MarkDecimal(_list.Amount[i] * GameManager.productList.Price[TabMode][Number-1]);
            }
        }
        else
        {
            for (int j = 0; j < 15; j++)
            {
                int i = j + UpperNumber;
                if (_list.Number[i] != -1)
                {
                    GameObject temp = Parent.transform.Find((j + 1).ToString()).gameObject;
                    temp.SetActive(true);
                    int TabMode = _list.TabMode[i];
                    int Number = _list.Number[i];
                    temp.transform.Find("ProductName").GetComponent<Text>().text = GameManager.productList.ProductName[TabMode][Number - 1];
                    temp.transform.Find("Amount").GetComponent<Text>().text = "数量 " + _list.Amount[i].ToString();
                    temp.transform.Find("Price").GetComponent<Text>().text = "￥" + GetComponent<GameManager>().MarkDecimal(_list.Amount[i] * GameManager.productList.Price[TabMode][Number - 1]);
                }
            }
        }

        if (UpperNumber > 0 && (MaximumType(_list) - UpperNumber)<=16)
        {
            UpButton.GetComponent<Button>().interactable = true;
        }
        else
        {
            UpButton.GetComponent<Button>().interactable = false;
        }

        if (MaximumType(_list) - UpperNumber > 15)
        {
            DownButton.GetComponent<Button>().interactable = true;
        }
        else
        {
            DownButton.GetComponent<Button>().interactable =false;
        }
    }

    private int MaximumType(GameManager.OrderList _list)
    {
        int i;
        for (i = 0; i < 24; i++)
        {
            if (_list.TabMode[i] == -1 && _list.Number[i] == -1)
                return i;
        }
        return 24;
    }

    public static void AddUpperNumber(int num)
    {
        UpperNumber += num;
    }
}
