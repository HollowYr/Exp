using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GridMap
{
    private int[,] gridArray;
    private TextMeshProUGUI[,] debugTextArray;
    private int width;
    private int height;
    private int cellSize;
    private Vector3 originPosition;

    private const int ERROR_VALUE = -1;

    public GridMap(int width, int height, int cellSize, Vector3 originPosition)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.originPosition = originPosition;

        gridArray = new int[width, height];
    }

    public void DebugDraw(Transform parent)
    {
        debugTextArray = new TextMeshProUGUI[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                DebugDrawCell(parent, x, y);

                Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x, y + 1), Color.white, 100f);
                Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x + 1, y), Color.white, 100f);
            }
        }

        Debug.DrawLine(GetWorldPosition(0, height), GetWorldPosition(width, height), Color.white, 100f);
        Debug.DrawLine(GetWorldPosition(width, 0), GetWorldPosition(width, height), Color.white, 100f);
    }

    private void DebugDrawCell(Transform parent, int x, int y)
    {
        Vector3 localCellPosition = new Vector3(x, 0, y) * cellSize;

        GameObject textObj = new GameObject("World_Text_" + x + y);
        textObj.transform.SetParent(parent);
        textObj.transform.localPosition = localCellPosition + cellSize * new Vector3(1, 0, 1) / 2;

        GameObject canvasObj = new GameObject("Canvas", typeof(Canvas));
        canvasObj.transform.SetParent(textObj.transform);
        canvasObj.AddComponent<BillboardCanvas>();

        Canvas canvas = canvasObj.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.transform.localPosition = Vector3.zero;
        canvas.GetComponent<RectTransform>().sizeDelta = Vector2.one * cellSize;

        GameObject text = new GameObject("TextMeshPro", typeof(TextMeshProUGUI));
        text.transform.SetParent(canvasObj.transform);

        RectTransform textRectTransform = text.GetComponent<RectTransform>();
        textRectTransform.anchorMin = new Vector2(0, 0);
        textRectTransform.anchorMax = new Vector2(1, 1);
        textRectTransform.pivot = new Vector2(0.5f, 0.5f);

        TextMeshProUGUI textMesh = text.GetComponent<TextMeshProUGUI>();
        textMesh.text = 0.ToString();
        textMesh.enableAutoSizing = true;
        textMesh.fontSizeMin = 0.01f;
        textMesh.verticalAlignment = VerticalAlignmentOptions.Middle;
        textMesh.horizontalAlignment = HorizontalAlignmentOptions.Center;

        debugTextArray[x, y] = textMesh;

        textRectTransform.sizeDelta = Vector2.zero;
        textRectTransform.localPosition = Vector3.zero;
    }

    private Vector3 GetWorldPosition(int x, int y) => new Vector3(x, 0, y) * cellSize + originPosition;

    public void GetGridCell(Vector3 worldPosition, out int x, out int y)
    {
        worldPosition -= originPosition;
        x = Mathf.FloorToInt(worldPosition.x / cellSize);
        y = Mathf.FloorToInt(worldPosition.z / cellSize);
    }

    public int GetCellValue(Vector3 worldPosition)
    {
        GetGridCell(worldPosition, out int x, out int y);
        return GetCellValue(x, y);
    }

    public void SetCellValue(Vector3 worldPosition, int value)
    {
        GetGridCell(worldPosition, out int x, out int y);
        SetCellValue(value, x, y);
    }
    private int GetCellValue(int x, int y)
    {
        if (x < 0 || x >= width || y < 0 || y >= height)
        {
#if UNITY_EDITOR
            //Debug.Log((x < 0) + " || " + (x >= width) + " || " + (y < 0) + " || " + (y >= height) + " || " );
            //Debug.LogError("X: " + x + "Y: " + y + "Width: " + width + "Height: " + height );
#endif
            return ERROR_VALUE;
        }

        return gridArray[x, y];
    }

    private void SetCellValue(int value, int x, int y)
    {
        Debug.Log("value: " + value + " x: " + x + " y: " + y);
        if (x < 0 || x >= width || y < 0 || y >= height || value == ERROR_VALUE) return;

        gridArray[x, y] = value;
        debugTextArray[x, y].text = value.ToString();
    }
}
