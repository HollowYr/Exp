using System;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField] Transform cameraTransform;
    [Space(10)]
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private ForceMode jumpForceMode = ForceMode.Impulse;
    [Space(10)]
    [SerializeField] private float movementSpeed = 10f;
    [SerializeField] private Transform rotationTransform;
    [SerializeField] private float rotationSpeed = 10f;

    Rigidbody rigidbody;
    IsGrounded isGrounded;
    SpringyMovement spring;
    ApplyGravity gravity;
    private void Start()
    {
        cameraTransform = Camera.main.transform;

        rigidbody = GetComponent<Rigidbody>();
        isGrounded = GetComponent<IsGrounded>();
        spring = GetComponent<SpringyMovement>();
        gravity = GetComponent<ApplyGravity>();
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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded.GetIsGrounded()) Jump();
    }


    private void FixedUpdate()
    {
        Move();
    }

    private void Jump()
    {
        rigidbody.AddForce(Vector3.up * jumpForce, jumpForceMode);
        isGrounded.onGroundedEvent?.Invoke(false);
    }

    private void Move()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 cameraForward = cameraTransform.forward;
        cameraForward.y = 0;
        cameraForward = cameraForward.normalized;

        Vector3 cameraRight = cameraTransform.right;
        cameraRight.y = 0;
        cameraRight = cameraRight.normalized;
        Vector3 movementDirection = vertical * cameraForward + horizontal * cameraRight;
        movementDirection = movementDirection.normalized;

        rigidbody.velocity = (new Vector3(0, rigidbody.velocity.y, 0) +
                                movementDirection * movementSpeed);

        if (movementDirection == Vector3.zero) return;

        Quaternion LookAtRotation = Quaternion.LookRotation(movementDirection);

        Quaternion LookAtRotationOnly_Y = Quaternion.Euler(rotationTransform.rotation.eulerAngles.x,
                                                           LookAtRotation.eulerAngles.y,
                                                           rotationTransform.rotation.eulerAngles.z);

        rotationTransform.rotation = Quaternion.RotateTowards(rotationTransform.rotation,
                                                              LookAtRotationOnly_Y,
                                                              rotationSpeed * Time.fixedDeltaTime);
    }
}