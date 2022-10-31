using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private ForceMode jumpForceMode = ForceMode.Impulse;
    [SerializeField] private float movementSpeed = 10f;

    Rigidbody newRigidbody;
    private void Start() => newRigidbody = GetComponent<Rigidbody>();

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) Jump();
        Move();
    }

    private void Jump()
    {
        newRigidbody.AddForce(Vector3.up * jumpForce, jumpForceMode);
    }

    private void Move()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        newRigidbody.velocity = (new Vector3(horizontal * movementSpeed, newRigidbody.velocity.y, vertical * movementSpeed));
    }
}