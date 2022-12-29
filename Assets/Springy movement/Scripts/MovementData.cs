using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/MovementData", order = 1)]
public class MovementData : ScriptableObject
{
    [SerializeField] internal float jumpForce = 10f;
    [SerializeField] internal ForceMode jumpForceMode = ForceMode.Impulse;
    [SerializeField] internal float movementSpeed = 10f;
    [SerializeField] internal float rotationSpeed = 10f;
}

