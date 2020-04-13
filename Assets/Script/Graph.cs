using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Graph : MonoBehaviour
{
    public Image Background;
    public GameObject DataPoint;
    public int sideWidth = 10;
    public int topWidth = 10;
    public int bottomWidth = 10;
    public float GridLineWidth = 3;
    public Color GridLineColor = new Color(0f, 0f, 0f, 1f);

    public class GraphData
    {
        public float Value = 0;
        public string HorizontalLabel=""; //横軸ラベル
        public string VerticalLabel=""; //縦軸ラベル
        public string DataDescription=""; //各データの情報
    }

    public void GenerateGraph(List<GraphData> data)
    {
        float sizeX = Background.gameObject.GetComponent<RectTransform>().sizeDelta.x - 2*sideWidth;
        float sizeY = Background.gameObject.GetComponent<RectTransform>().sizeDelta.y - topWidth - bottomWidth;
        float Width = data.Count + 1;

        float MaximumValue = maxAndMinValue(data).x;
        float MinimumValue = maxAndMinValue(data).y;
        float Height = Mathf.Ceil(MaximumValue - MinimumValue);

        Vector2 Division = new Vector2(sizeX / Width, sizeY / Height);

        //GridLine
        
    }
    
    private void DrawLine(float x1, float y1, float x2, float y2, float width, GameObject Parent)
    {
        float x = (x2 + x1)/2f;
        float y = (y2 + y1)/2f;
        float length = Mathf.Sqrt((x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1));
        GameObject line = new GameObject("Line", typeof(Image));
        RectTransform rectTran = line.GetComponent<RectTransform>();
        line.transform.SetParent(Parent.transform);

        rectTran.localPosition = new Vector2(x, y);
        rectTran.sizeDelta = new Vector2(length, width);
        Mathf.Atan2(y2 - y1, x2 - x1);
    }

    private Vector2 maxAndMinValue(List<GraphData> data)
    {
        float max;
        float min;

        if (data.Count == 0)
        {
            max = 0;
            min = 0;
        }
        else
        {
            max = data[0].Value;
            min = data[0].Value;
            foreach (var record in data)
            {
                if (record.Value > max)
                    max = record.Value;
                if (record.Value < min)
                    min = record.Value;
            }
        }
        if (min > 0)
            min = 0;

        return new Vector2(max, min);
    }

}
