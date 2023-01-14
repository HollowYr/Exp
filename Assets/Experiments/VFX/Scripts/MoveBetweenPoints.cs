#define DEBUG
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class MoveBetweenPoints : ImprovedMonoBehaviour
{
    [SerializeField] private VisualEffect VFX;
    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;
    [SerializeField] private float time;
    [ShowNonSerializedField]
    private float timer = 0;
    void Update()
    {
        timer += Time.deltaTime;
        timer %= time;
        if (timer <= .1) VFX.Play();
        transform.position = Vector3.Lerp(pointA.position, pointB.position, timer);
    }

    private void OnDrawGizmos()
    {
        Update();
    }
}

