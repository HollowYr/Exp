using UnityEngine;
using System;

public class Slope : ImprovedMonoBehaviour
{
    [SerializeField] private Transform playerModel;
    [SerializeField] private float slopeLimit = 45f;
    [SerializeField] private float slopeSpeed = 45f;
    private Rigidbody rigidbody;
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        
    }
    void Update()
    {
        Ray ray = new Ray(playerModel.position, playerModel.up * -1);
        if (Physics.Raycast(ray, out RaycastHit hit, 5f))
        {
            Debug.DrawRay(hit.point, hit.normal * 2);
            var angle = Vector3.Angle(Vector3.up, hit.normal);
            if (angle > slopeLimit)
            {
                var normal = hit.normal;
                var yInverse = 1f - normal.y;
                Vector3 velocity = Vector3.zero;
                velocity.x += yInverse * normal.x;
                velocity.z += yInverse * normal.z;
                rigidbody.velocity += velocity * slopeSpeed;
            }
        }
    }
}