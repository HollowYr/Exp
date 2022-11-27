using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotation : MonoBehaviour
{
    [SerializeField] private float rotationPower = 10f;
    // Update is called once per frame
    void Update()
    {
        float horizontal = Input.GetAxis("Mouse X");
        float vertical = Input.GetAxis("Mouse Y") * -1f;

        transform.rotation *= Quaternion.AngleAxis(horizontal * rotationPower, Vector3.up);
        transform.rotation *= Quaternion.AngleAxis(vertical * rotationPower, Vector3.right);

        Vector3 angles = transform.localEulerAngles;
        angles.z = 0;

        float angle = angles.x;
        if (angle > 180 && angle < 340)
        {
            angles.x = 340;
        }
        else if (angle < 180 && angle > 40)
        {
            angles.x = 40;
        }

        transform.localEulerAngles = angles;
    }
}
