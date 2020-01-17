using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class OrderListComponent : MonoBehaviour
{
    public GameObject Parent;
    public GameObject UpButton;
    public GameObject DownButton;
    public int UpperNumber;

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
            for (int i = 0; i < 15; i++)
            {
                //注文リストが画面いっぱいに表示された時の処理
            }
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
}
