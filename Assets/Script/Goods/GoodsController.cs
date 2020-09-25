using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GoodsController : MonoBehaviour
{
    private int GoodsNumber = 0;
    public int ButtonMode;
    public Button GoodsData;
    public Button TabName;
    public GameObject GoodsUI;
    public GameObject TabNameUI;
    public GameObject 商品リスト;
    public GameObject 商品リストテンプレート;
    public Dropdown CategoryUI;

    void Start()
    {
        CategoryUI = GoodsUI.transform.Find("商品情報").Find("Category").GetComponent<Dropdown>();
        GoodsNumber = -1;
        ButtonMode = 0;
        ButtonPressed(ButtonMode);
        ClearForm();

        CategoryUI.options.Clear();
        CategoryUI.options.Add(new Dropdown.OptionData { text = GetComponent<DataLoader>().LoadConfig().TabName[0] });
        CategoryUI.options.Add(new Dropdown.OptionData { text = GetComponent<DataLoader>().LoadConfig().TabName[1] });
        CategoryUI.options.Add(new Dropdown.OptionData { text = GetComponent<DataLoader>().LoadConfig().TabName[2] });
        CategoryUI.options.Add(new Dropdown.OptionData { text = GetComponent<DataLoader>().LoadConfig().TabName[3] });
    }


    public void ButtonPressed(int num)
    {
        ButtonMode = num;
        if(num>=0 && num <= 3)
        {
            GoodsUI.SetActive(true);
            TabNameUI.SetActive(false);
            setGoodsData(GoodsNumber);

            CategoryUI.options.Clear();
            CategoryUI.options.Add(new Dropdown.OptionData { text = GetComponent<DataLoader>().LoadConfig().TabName[0] });
            CategoryUI.options.Add(new Dropdown.OptionData { text = GetComponent<DataLoader>().LoadConfig().TabName[1] });
            CategoryUI.options.Add(new Dropdown.OptionData { text = GetComponent<DataLoader>().LoadConfig().TabName[2] });
            CategoryUI.options.Add(new Dropdown.OptionData { text = GetComponent<DataLoader>().LoadConfig().TabName[3] });
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
        DataLoader.List record;
        if(GoodsNumber == -1)
        {
            record = new DataLoader.List();
        }
        else
        {
            record = tempList[GoodsNumber];
        }

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
        CategoryUI.value = record.Category;
        CategoryUI.RefreshShownValue();
        temp.Find("Name").GetComponent<InputField>().text = record.Name;
        temp.Find("Price").GetComponent<InputField>().text = record.Price.ToString();
        temp.Find("Stock").GetComponent<InputField>().text = record.Stock.ToString();
        temp.Find("Available").GetComponent<Toggle>().isOn = record.Available;
        temp.Find("ImagePath").GetComponent<InputField>().text = record.ImagePath;
        temp.Find("ID").GetComponent<InputField>().text = record.ID;
        string dir = "image/" + record.ImagePath;
        SetImage(dir);
    }

    private void SetImage(string dir)
    {
        Transform temp = GoodsUI.transform.Find("商品情報");
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
        DataLoader.SaveList(GetComponent<DataLoader>().RemoveListDuplicate(tempList));
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

    private DataLoader.List GetDataFromForm()
    {
        Transform temp = GoodsUI.transform.Find("商品情報");
        return new DataLoader.List
        {
            Category = CategoryUI.value,
            Name = temp.Find("Name").GetComponent<InputField>().text,
            Price = Number.ToNumber(temp.Find("Price").GetComponent<InputField>().text),
            Stock = Number.ToNumber(temp.Find("Stock").GetComponent<InputField>().text),
            Available = temp.Find("Available").GetComponent<Toggle>().isOn,
            ImagePath = temp.Find("ImagePath").GetComponent<InputField>().text,
            ID = temp.Find("ID").GetComponent<InputField>().text
        };
    }

    public void addGoodsData()
    {
        var tempList = DataLoader.LoadList();
        tempList.Add(GetDataFromForm());
        DataLoader.SaveList(GetComponent<DataLoader>().RemoveListDuplicate(tempList));
        ButtonPressed(0);
    }

    public void removeGoodsData()
    {
        try
        {
            var tempList = DataLoader.LoadList();
            var t = tempList[GoodsNumber];
            Transform temp = GoodsUI.transform.Find("商品情報");

            if (t.Name == temp.Find("Name").GetComponent<InputField>().text &&
                t.Category == CategoryUI.value &&
                t.Price.ToString() == temp.Find("Price").GetComponent<InputField>().text &&
                t.Stock.ToString() == temp.Find("Stock").GetComponent<InputField>().text &&
                t.Available == temp.Find("Available").GetComponent<Toggle>().isOn &&
                t.ImagePath == temp.Find("ImagePath").GetComponent<InputField>().text &&
                t.ID == temp.Find("ID").GetComponent<InputField>().text
                )
            {
                tempList.RemoveAt(GoodsNumber);
                DataLoader.SaveList(tempList);
                ClearForm();
                ButtonPressed(0);
            }
        }
        catch
        {
            Debug.LogWarning("Number " + GoodsNumber.ToString() + " was not exist.");
        }
    }

    public void ClearForm()
    {
        Transform temp = GoodsUI.transform.Find("商品情報");
        CategoryUI.value=0;
        temp.Find("Name").GetComponent<InputField>().text = "";
        temp.Find("Price").GetComponent<InputField>().text = "";
        temp.Find("Stock").GetComponent<InputField>().text = "";
        temp.Find("Available").GetComponent<Toggle>().isOn = true;
        temp.Find("ImagePath").GetComponent<InputField>().text = "";
        temp.Find("ID").GetComponent<InputField>().text = "";

        GoodsNumber = -1;
        string dir = "image/" + "";
        SetImage(dir);
    }

    public void Swap(int num)
    {
        try
        {
            if (num == 1) //UP
            {
                var tempList = DataLoader.LoadList();
                var tmp = tempList[GoodsNumber];
                tempList[GoodsNumber] = tempList[GoodsNumber-1];
                tempList[GoodsNumber-1] = tmp;
                DataLoader.SaveList(tempList);
                GoodsNumber -= 1;
                ButtonPressed(0);
            }
            else if (num == -1) //DOWN
            {
                var tempList = DataLoader.LoadList();
                var tmp = tempList[GoodsNumber];
                tempList[GoodsNumber] = tempList[GoodsNumber + 1];
                tempList[GoodsNumber + 1] = tmp;
                DataLoader.SaveList(tempList);
                GoodsNumber += 1;
                ButtonPressed(0);
            }
        }
        catch
        {
            Debug.LogWarning("It can't move.");
        }
    }
}
