using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpringyMovement : ImprovedMonoBehaviour
{
    [SerializeField] private float desiredFloatHeight = 1.5f;
    [SerializeField] private float uprightSpringStrength = 50f;
    [SerializeField] private float uprightSpringDamper = 1f;
    [SerializeField] private float tension = 10f;
    [SerializeField] private float rideSpringDamer = 1f;

    private float rayLength = float.PositiveInfinity;
    private Rigidbody springedRigidbody;

    private void Start() => springedRigidbody = GetComponent<Rigidbody>();

    void FixedUpdate()
    {
        Float();
        //RotationTorque();
    }

    private void RotationTorque()
    {
        Quaternion characterCurrent = transform.rotation;
        Quaternion toGoal = ShortestRotation(Quaternion.LookRotation(Vector3.forward), characterCurrent);

        Vector3 rotAxis;
        float rotDegrees;

        toGoal.ToAngleAxis(out rotDegrees, out rotAxis);
        rotAxis.Normalize();

        float rotRadians = rotDegrees * Mathf.Deg2Rad;
        springedRigidbody.AddTorque((rotAxis * (rotRadians * uprightSpringStrength)) - (base.GetComponent<Rigidbody>().angularVelocity * uprightSpringDamper));
    }

    public static Quaternion ShortestRotation(Quaternion a, Quaternion b)
    {
        return (Quaternion.Dot(a, b) < 0) ?
             a * Quaternion.Inverse(Multiply(b, -1)) :
             a * Quaternion.Inverse(b);
    }

    public static Quaternion Multiply(Quaternion input, float scalar)
    {
        return new Quaternion(input.x * scalar, input.y * scalar, input.z * scalar, input.w * scalar);
    }

    private void Float()
    {
        Color hitDebugColor = Color.red;
        Ray ray = new Ray(transform.position, transform.up * -1);
        if (Physics.Raycast(ray, out RaycastHit hit, rayLength))
        {
            hitDebugColor = Color.green;

            Vector3 vel = springedRigidbody.velocity;
            Vector3 rayDir = ray.direction;

            Vector3 otherVel = Vector3.zero;
            Rigidbody hitBody = hit.rigidbody;
            if (hitBody != null)
            {
                otherVel = hitBody.velocity;
            }

            float rayDirVel = Vector3.Dot(rayDir, vel);
            float otherDirVel = Vector3.Dot(rayDir, otherVel);

            float relVel = rayDirVel - otherDirVel;

            float x = hit.distance - desiredFloatHeight;
            float springForce = (x * tension) - (relVel * rideSpringDamer);
            springedRigidbody.AddForce(rayDir * springForce, ForceMode.Acceleration);
        }
        Debug.DrawLine(ray.origin, new Vector3(ray.origin.x, ray.origin.y - 100, ray.origin.z), hitDebugColor, Time.fixedDeltaTime);
    }
}
