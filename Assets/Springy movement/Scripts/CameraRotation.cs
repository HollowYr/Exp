using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class CameraRotation : MonoBehaviour
{
    [SerializeField] private float rotationPower = 10f;
    [SerializeField] private readonly Vector2 angleClamp = new Vector2(40, 340);

    float horizontal;
    float vertical;

    private void Start()
    {
        Cursor.visible = false;
    }

    private void FixedUpdate()
    {
        transform.rotation *= Quaternion.AngleAxis(horizontal * rotationPower, Vector3.up);
        transform.rotation *= Quaternion.AngleAxis(vertical * rotationPower, Vector3.right);

        Vector3 angles = transform.localEulerAngles;
        angles.z = 0;

        float angle = angles.x;
        if (angle > 180 && angle < angleClamp.y)
        {
            angles.x = angleClamp.y;
        }
        else if (angle < 180 && angle > angleClamp.x)
        {
            angles.x = angleClamp.x;
        }

        transform.localEulerAngles = angles;

    }
    void Update()
    {
        horizontal = Input.GetAxis("Mouse X");
        vertical = Input.GetAxis("Mouse Y") * -1f;
    }
}
