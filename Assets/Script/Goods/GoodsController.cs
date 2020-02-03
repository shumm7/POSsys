using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GoodsController : MonoBehaviour
{
    private int GoodsNumber;
    public int ButtonMode;
    public Button GoodsData;
    public Button TabName;
    public GameObject GoodsUI;
    public GameObject TabNameUI;
    public GameObject 商品リスト;
    public GameObject 商品リストテンプレート;

    void Start()
    {
        GoodsNumber = 0;
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
            setGoodsData(GoodsNumber);
        }
        else if (num==4)
        {
            GoodsUI.SetActive(false);
            TabNameUI.SetActive(true);

            var tempList = DataLoader.LoadList();
            for (int i = 0; i < 4; i++)
            {
                TabNameUI.transform.Find("Tab" + (i + 1).ToString()).GetComponent<InputField>().text = GetComponent<DataLoader>().LoadConfig().TabName[i];
            }
        }
    }

    public void setGoodsData(int number)
    {
        GoodsNumber = number;
        var tempList = DataLoader.LoadList();
        var record = tempList[GoodsNumber];

        foreach (Transform l in 商品リスト.transform)
        {
            Destroy(l.gameObject);
        }

        int count = 0;
        foreach (var l in tempList)
        {
            var tempObject = Instantiate(商品リストテンプレート, 商品リスト.transform);
            tempObject.name = count.ToString();
            tempObject.transform.Find("Name").GetComponent<Text>().text = "[" + (l.Category+1).ToString() + "] "+l.Name;
            tempObject.SetActive(true);
            count++;
        }

        Transform temp = GoodsUI.transform.Find("商品情報");
        Dropdown CategoryUI = temp.Find("Category").GetComponent<Dropdown>();
        CategoryUI.options.Clear();
        CategoryUI.options.Add(new Dropdown.OptionData { text = GetComponent<DataLoader>().LoadConfig().TabName[0] });
        CategoryUI.options.Add(new Dropdown.OptionData { text = GetComponent<DataLoader>().LoadConfig().TabName[1] });
        CategoryUI.options.Add(new Dropdown.OptionData { text = GetComponent<DataLoader>().LoadConfig().TabName[2] });
        CategoryUI.options.Add(new Dropdown.OptionData { text = GetComponent<DataLoader>().LoadConfig().TabName[3] });
        CategoryUI.value = record.Category;
        CategoryUI.RefreshShownValue();
        temp.Find("Name").GetComponent<InputField>().text = record.Name;
        temp.Find("Price").GetComponent<InputField>().text = record.Price.ToString();
        temp.Find("Stock").GetComponent<InputField>().text = record.Stock.ToString();
        temp.Find("Available").GetComponent<Toggle>().isOn = record.Available;
        temp.Find("ImagePath").GetComponent<InputField>().text = record.ImagePath;
        temp.Find("ID").GetComponent<InputField>().text = record.ID;

        string dir = "image/" + record.ImagePath;
        if (GetComponent<DataLoader>().checkExist(dir))
        {
            RawImage img = temp.Find("RawImage").GetComponent<RawImage>();
            byte[] bytes = System.IO.File.ReadAllBytes(dir);
            Texture2D texture = new Texture2D(500, 500);
            texture.LoadImage(bytes);
            img.texture = texture;
            img.color = new Color(1f, 1f, 1f, 1f);
        }
        else
        {
            RawImage img = temp.Find("RawImage").GetComponent<RawImage>();
            img.texture = null;
            img.color = new Color(100f / 255f, 100f / 255f, 100f / 255f, 1f);
        }
    }

    public void saveGoodsData()
    {
        DataLoader.List data = new DataLoader.List();
        var tempList = DataLoader.LoadList();

        Transform temp = GoodsUI.transform.Find("商品情報");
        data.Category = temp.Find("Category").GetComponent<Dropdown>().value;
        data.Name = temp.Find("Name").GetComponent<InputField>().text;
        data.Price = Number.ToNumber(temp.Find("Price").GetComponent<InputField>().text);
        data.Stock = Number.ToNumber(temp.Find("Stock").GetComponent<InputField>().text);
        data.Available = temp.Find("Available").GetComponent<Toggle>().isOn;
        data.ImagePath = temp.Find("ImagePath").GetComponent<InputField>().text;
        data.ID = temp.Find("ID").GetComponent<InputField>().text;

        tempList[GoodsNumber] = data;
        DataLoader.SaveList(tempList);
        setGoodsData(GoodsNumber);
    }

    public void saveTabName()
    {
       var tempList = GetComponent<DataLoader>().LoadConfig();
        for (int i = 0; i < 4; i++)
        {
            tempList.TabName[i] = TabNameUI.transform.Find("Tab" + (i + 1).ToString()).GetComponent<InputField>().text;
        }
        GetComponent<DataLoader>().SaveConfig(tempList);
    }
}
