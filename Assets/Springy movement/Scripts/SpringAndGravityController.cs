#define DEBUG
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpringAndGravityController : ImprovedMonoBehaviour
{
    void Start()
    {
        SpringyMovement spring = GetComponent<SpringyMovement>();
        ApplyGravity gravity = GetComponent<ApplyGravity>();
        IsGrounded isGrounded = GetComponent<IsGrounded>();
        isGrounded.onGroundedEvent += b =>
        {
            if (b)
            {
                spring.enabled = true;
                gravity.enabled = false;
            }
            else
            {
                spring.enabled = false;
                gravity.enabled = true;
            }
        };
    }

}

