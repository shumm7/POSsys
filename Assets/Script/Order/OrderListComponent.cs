using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class OrderListComponent : MonoBehaviour
{
    GameObject GameManager;

    public void 削除()
    {
        GameManager = transform.parent.parent.parent.parent.parent.parent.parent.Find("Controller").gameObject;
        GameManager.GetComponent<GameManager>().注文削除(int.Parse(gameObject.name));
    }
}
