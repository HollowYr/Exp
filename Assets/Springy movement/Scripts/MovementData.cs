using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/MovementData", order = 1)]
public class MovementData : ScriptableObject
{
    [Foldout("Movement"), SerializeField] internal float jumpForce = 10f;
    [Foldout("Movement"), SerializeField] internal ForceMode jumpForceMode = ForceMode.Impulse;
    [Foldout("Movement"), SerializeField] internal float movementSpeed = 10f;
    [Foldout("Movement"), SerializeField] internal float rotationSpeed = 10f;

    [Foldout("Wallrun"), SerializeField] internal float distanceToWall = .5f;
    [Foldout("Wallrun"), SerializeField] internal float stickToWallPower = 50f;
    [Foldout("Wallrun"), SerializeField] internal float maxTime = 5f;
    [Foldout("Wallrun"), SerializeField] internal float additionalSpeed = 50f;
    [Foldout("Wallrun"), SerializeField, Layer] internal int layerWall;

    [Foldout("Player"), SerializeField] internal float playerRadius = 1f;
}

