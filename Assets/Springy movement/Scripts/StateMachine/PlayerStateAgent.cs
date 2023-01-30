using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateAgent : ImprovedMonoBehaviour
{
    [Header("Transforms")]
    [SerializeField] internal Transform cameraTransform;
    [SerializeField] internal Transform playerModel;
    [SerializeField] internal MovementData movementData;
    [SerializeField] internal PlayerStateMachine stateMachine;
    [SerializeField] internal Animator animator;
    [SerializeField] private PlayerStateID initialState;
    internal Rigidbody rigidbody;
    private CapsuleCollider collider;
    private IsGrounded isGrounded;
    internal bool isGroundedState = false;
    internal bool allowGroundedStateChange = true;
    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
        collider = GetComponentInChildren<CapsuleCollider>();
        rigidbody = GetComponent<Rigidbody>();
        isGrounded = GetComponent<IsGrounded>();

        stateMachine = new PlayerStateMachine(this);
        stateMachine.RegisterState(new PlayerState_Idle());
        stateMachine.RegisterState(new PlayerState_InAir());
        stateMachine.RegisterState(new PlayerState_Walk());
        stateMachine.RegisterState(new PlayerState_Wallrun());
        stateMachine.RegisterState(new PlayerState_RailGrind());
        stateMachine.ChangeState(initialState);


        isGrounded.onGroundedEvent += b =>
        {
            if (isGroundedState == b) return;

            isGroundedState = b;
            if (allowGroundedStateChange == false) return;

            if (b)
            {
                Debug.Log(maxY - debugYPos);
                maxY = 0;
            }
            stateMachine.ChangeState((b) ? PlayerStateID.Idle : PlayerStateID.InAir);
        };
    }
    private void Update()
    {
        if (stateMachine == null) return;

        stateMachine.Update();
    }
    private void FixedUpdate()
    {
        if (stateMachine == null) return;
        stateMachine.FixedUpdate();
    }
    internal Vector3 GetPlayerMovementDirection()
    {
        float horizontal = UnityLegacy.InputHorizontal();
        float vertical = UnityLegacy.InputVertical();

        Vector3 cameraForward = cameraTransform.forward;
        cameraForward.y = 0;
        cameraForward = cameraForward.normalized;

        Vector3 cameraRight = cameraTransform.right;
        cameraRight.y = 0;
        cameraRight = cameraRight.normalized;
        Vector3 movementDirection = vertical * cameraForward + horizontal * cameraRight;
        movementDirection = movementDirection.normalized;
        return movementDirection;
    }
    public bool GetIsGrounded() => isGroundedState;
    public void ModifyColliderRaius(float newRadius) => collider.radius = newRadius;
    internal void Jump()
    {
        Vector3 movementDirection = GetPlayerMovementDirection();
        movementDirection = movementDirection.normalized;
        movementDirection.y = 1;
        Jump(movementDirection);
    }
    internal void Jump(Vector3 direction)
    {
        Vector3 jumpForce = direction;
        jumpForce.x *= movementData.jumpSideForce;
        jumpForce.z *= movementData.jumpSideForce;
        jumpForce.y *= movementData.jumpUpForce;

        Vector3 velocity = rigidbody.velocity;
        velocity.y = 0;
        rigidbody.velocity = velocity;

        rigidbody.AddForce(jumpForce, movementData.jumpForceMode);
        isGrounded.onGroundedEvent?.Invoke(false);
        maxY = 0;
        debugYPos = transform.position.y;
    }
    float maxY = 0;
    float debugYPos;
    private void OnDrawGizmos()
    {
        if (transform.position.y > maxY)
        {
            maxY = transform.position.y;
        }
        if (stateMachine == null) return;
        stateMachine.OnDrawGizmos();
    }
}
