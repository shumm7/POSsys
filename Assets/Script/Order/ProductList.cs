using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProductList : MonoBehaviour
{
    public static int 各カテゴリーの商品数取得(List<DataLoader.List> _list, int category)
    {
        int count = 0;
        foreach (DataLoader.List record in _list)
        {
            if(record.Category == category)
            {
                count++;
            }
        }
        return count;
    }

    public static int getListLength(List<DataLoader.List> _list)
    {
        return _list.Count;
    }

    public static List<DataLoader.List> 各カテゴリーのリスト取得(List<DataLoader.List> _list, int category)
    {
        List<DataLoader.List> res = new List<DataLoader.List>();
        foreach (DataLoader.List record in _list)
        {
            if (record.Category == category)
            {
                res.Add(record);
            }
        }
        return res;
    }
}
