using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class OrderButton : MonoBehaviour
{
    public GameObject Controller;
    private int TabMode;

    // Start is called before the first frame update
    void Start()
    {
        TabMode = Controller.GetComponent<GameManager>().TabMode;
        SetImage(TabMode, getOwnButtonNumber());
    }

    public void TabChanged(int _TabMode, int num)
    {
        TabMode = _TabMode;
        SetImage(_TabMode, num);
    }

    public void ButtonPressed(int num)
    {

    }

    public void SetImage(int TabMode, int num)
    {
        DataLoader.List ProductList;
        ProductList = GetComponent<DataLoader>().LoadList();

        if (ProductList.Available[TabMode][num - 1])
        {
            gameObject.SetActive(true);
            gameObject.transform.Find("Text").GetComponent<Text>().text = ProductList.ProductName[TabMode][num - 1];
            string dir = "list/" + (TabMode + 1).ToString() + "/" + num.ToString() + ".png";
            if (GetComponent<DataLoader>().checkExist(dir))
            {
                RawImage img = gameObject.transform.Find("RawImage").GetComponent<RawImage>();
                byte[] bytes = File.ReadAllBytes(dir);
                Texture2D texture = new Texture2D(500, 500);
                texture.LoadImage(bytes);
                img.texture = texture;
                img.color = new Color(1f, 1f, 1f, 1f);
            }
            else
            {
                RawImage img = gameObject.transform.Find("RawImage").GetComponent<RawImage>();
                img.texture = null;
                img.color = new Color(100f / 255f, 100f / 255f, 100f / 255f, 1f);
            }
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    public int getOwnButtonNumber()
    {
        return int.Parse(gameObject.name);
    }
}
