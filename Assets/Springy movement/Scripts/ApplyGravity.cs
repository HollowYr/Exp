using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplyGravity : ImprovedMonoBehaviour
{
    [SerializeField] float gravityForce = 9.8f;
    [SerializeField] private MovementData movementData;
    private Rigidbody body;
    private void Start()
    {
        body = GetComponent<Rigidbody>();
        gravityForce = movementData.gravity;
    }
    void FixedUpdate()
    {
        body.velocity += Vector3.down * gravityForce * Time.fixedDeltaTime;
    }
}
