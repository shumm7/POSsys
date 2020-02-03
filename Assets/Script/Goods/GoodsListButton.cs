using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoodsListButton : MonoBehaviour
{
    public void Clicked商品データ()
    {
        GoodsController Controller = gameObject.transform.parent.parent.parent.parent.parent.Find("Controller").GetComponent<GoodsController>();
        Controller.setGoodsData(Number.ToNumber(transform.name));
    }
}
