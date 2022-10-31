using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestGrid : MonoBehaviour
{
    GridMap map;
    void Start()
    {
        map = new GridMap(5, 5, 1, transform.position);
        map.DebugDraw(transform);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {

                int startValue = map.GetCellValue(hit.point);
                Debug.Log("start value: " + startValue);
                //Debug.Log("world: " + hit.point);
                //map.GetGridCell(hit.point, out int x, out int y);
                //Debug.Log("grid: x: " + x + " y: " + y);
                map.SetCellValue(hit.point, startValue+1);
            }
        }
    }
}
