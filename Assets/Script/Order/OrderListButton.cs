using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderListButton : MonoBehaviour
{
    public void OnClicked()
    {
        Transform ControllerTransform = transform.parent.parent.parent.parent.Find("Controller");
        GameManager.OrderList tempList = ControllerTransform.GetComponent<GameManager>().orderList;
        int i = int.Parse(transform.name) + ControllerTransform.GetComponent<OrderListComponent>().UpperNumber - 1;
        ControllerTransform.GetComponent<GameManager>().RemoveOrder(tempList.TabMode[i], tempList.Number[i]);
    }
}
