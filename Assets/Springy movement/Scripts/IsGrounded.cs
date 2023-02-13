#define DEBUG
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class IsGrounded : ImprovedMonoBehaviour
{
    [SerializeField] private float playerHeight = 2f;
    [Layer, SerializeField] private int layerWalkable;
    [Layer, SerializeField] private int layerGrindRails;
    int layersToWalkBy;
    private bool isGrounded = false;
    public Action<bool> onGroundedEvent;

    void Start()
    {
        layersToWalkBy = (1 << layerGrindRails) | (1 << layerWalkable);
    }

    private void FixedUpdate()
    {
        Ray ray = new Ray(transform.position, transform.up * -1);
        if (Physics.Raycast(ray, out RaycastHit hit, playerHeight, layersToWalkBy, QueryTriggerInteraction.Ignore))
        {
#if DEBUG
            debugHit = hit.point;
            debugColor = Color.green;
#endif
            isGrounded = true;
            onGroundedEvent?.Invoke(isGrounded);
        }
        else
        {
#if DEBUG
            debugColor = Color.red;
#endif
            isGrounded = false;
            onGroundedEvent?.Invoke(isGrounded);
        }
    }

#if DEBUG
    Vector3 debugHit;
    Color debugColor;
    private void OnDrawGizmos()
    {
        if (debugHit == null) return;
        Gizmos.color = debugColor;
        Gizmos.DrawSphere(transform.position, .1f);
        Gizmos.DrawSphere(debugHit, .1f);
    }
#endif
}
