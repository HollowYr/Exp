using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsGrounded : MonoBehaviour
{
    [SerializeField] private float playerHeight = 2f;
    private bool isGrounded = false;
    public Action<bool> onGroundedEvent;
    public bool GetIsGrounded() => isGrounded;
    private void Update()
    {
        Ray ray = new Ray(transform.position, transform.up * -1);
        if (Physics.Raycast(ray, out RaycastHit hit, playerHeight))
        {
            debugHit = hit.point;
            debugColor = Color.green;
            //if (isGrounded == true) return;
            isGrounded = true;
            onGroundedEvent?.Invoke(isGrounded);
        }
        else
        {
            debugColor = Color.red;
            //if (isGrounded == false) return;
            isGrounded = false;
            onGroundedEvent?.Invoke(isGrounded);
        }
    }
    Vector3 debugHit;
    Color debugColor;
    private void OnDrawGizmos()
    {
        if (debugHit == null) return;
        Gizmos.color = debugColor;
        Gizmos.DrawSphere(debugHit, .1f);
    }

}
