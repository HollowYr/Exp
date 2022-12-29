using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wallrun : MonoBehaviour
{
    [SerializeField] private float distanceToWall = .5f;
    [SerializeField] private float stickToWallPower = 50f;
    [SerializeField] private float maxTime = 5f;
    [SerializeField] private float additionalSpeed = 50f;
    [SerializeField] private Transform playerModel;

    private float timer = 0;
    private Rigidbody rigidbody;
    private IsGrounded grounded;
    private ApplyGravity applyGravity;
    private bool isGrounded;
    private Vector3 force;
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        grounded = GetComponent<IsGrounded>();
        applyGravity = GetComponent<ApplyGravity>();

        grounded.onGroundedEvent += value =>
        {
            isGrounded = value;
            if (isGrounded == true) timer = 0;
        };
    }

    // Update is called once per frame
    void Update()
    {
        if (isGrounded)
        {
            rigidbody.constraints = RigidbodyConstraints.None;
            rigidbody.constraints = RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY;
            return;
        }

        force = Vector3.zero;

        Ray ray = new Ray(playerModel.position, playerModel.right);
        if (Physics.Raycast(ray, out RaycastHit hit, distanceToWall))
        {
            force = (hit.point - (hit.point + hit.normal)) * stickToWallPower * Time.fixedDeltaTime;
            DebugLegacy.DebugDrawLine(transform.position, transform.position + force, Color.green, 10f);
            Debug.DrawLine(hit.point, hit.point + hit.normal, Color.green, Time.deltaTime);
        }

        ray = new Ray(playerModel.position, -playerModel.right);
        if (Physics.Raycast(ray, out hit, distanceToWall))
        {
            force = (hit.point - (hit.point + hit.normal)) * stickToWallPower * Time.fixedDeltaTime;
            DebugLegacy.DebugDrawLine(transform.position, transform.position + force, Color.green, 10f);
            Debug.DrawLine(hit.point, hit.point + hit.normal, Color.green, Time.deltaTime);
        }

    }

    private void FixedUpdate()
    {
        if (isGrounded || force == Vector3.zero || timer > maxTime || Input.GetAxis("Vertical") <= 0)
        {
            if (!isGrounded)
                applyGravity.enabled = true;
            rigidbody.constraints = RigidbodyConstraints.None;
            rigidbody.constraints = RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY;
            return;
        }
        timer += Time.fixedDeltaTime;
        rigidbody.constraints = RigidbodyConstraints.FreezePositionY;
        Vector2 input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).normalized;
        rigidbody.velocity += force * input.x * input.y + playerModel.forward * input.y * additionalSpeed;
        //rigidbody.AddForce(Vector3.up * 68.6f * 2f * Time.fixedDeltaTime);
    }
}
