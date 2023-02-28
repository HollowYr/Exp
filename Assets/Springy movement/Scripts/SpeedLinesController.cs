#define DEBUG
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class SpeedLinesController : ImprovedMonoBehaviour
{
    [SerializeField] private MovementData movementData;
    [SerializeField] private Rigidbody playerRigidbody;
    [SerializeField] private float minCircleRadius = 7.8f;
    [SerializeField] private float maxCircleRadius = 8.8f;

    float minSpeed;
    float maxSpeed;
    ParticleSystem speedLines;
    ParticleSystem.ShapeModule speedLinesShape;
    void Start()
    {
        minSpeed = movementData.movementSpeed;
        minSpeed *= minSpeed;

        maxSpeed = Mathf.Max(movementData.wallrunSpeed, movementData.railsMovementSpeed);
        maxSpeed *= maxSpeed;

        speedLines = GetComponent<ParticleSystem>();
        speedLinesShape = speedLines.shape;
    }

    void Update()
    {
        float speed = playerRigidbody.velocity.sqrMagnitude;
        float radius = speed.Remap(minSpeed, maxSpeed, maxCircleRadius, minCircleRadius);
        speedLinesShape.radius = radius;

        if (radius < maxCircleRadius)
            speedLines.Play();
        else
            speedLines.Stop();
    }
}

