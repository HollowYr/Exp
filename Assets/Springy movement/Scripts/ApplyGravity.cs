using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplyGravity : MonoBehaviour
{
    [SerializeField] float gravityForce = 9.8f;
    private Rigidbody body;
    private void Start() => body = GetComponent<Rigidbody>();

    void FixedUpdate()
    {
        body.velocity += Vector3.down * gravityForce * Time.fixedDeltaTime;
    }
}
