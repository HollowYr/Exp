using UnityEngine;
using System;

public class Slope : ImprovedMonoBehaviour
{
    [SerializeField] Transform playerModel;
    void Update()
    {
        Ray ray = new Ray(playerModel.position, playerModel.up * -1);
        if (Physics.Raycast(ray, out RaycastHit hit, 5f))
        {
            Debug.DrawRay(hit.point, hit.normal * 2);
        }
    }
}