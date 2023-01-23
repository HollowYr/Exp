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
    [SerializeField] private PlayerStateID initialState;

    internal Rigidbody rigidbody;
    private IsGrounded isGrounded;
    internal bool isGroundedState = false;
    private void Start()
    {
        stateMachine = new PlayerStateMachine(this);
        stateMachine.RegisterState(new PlayerState_Idle());
        stateMachine.RegisterState(new PlayerState_InAir());
        stateMachine.RegisterState(new PlayerState_Walk());
        stateMachine.RegisterState(new PlayerState_Wallrun());
        stateMachine.ChangeState(initialState);

        rigidbody = GetComponent<Rigidbody>();
        isGrounded = GetComponent<IsGrounded>();
        isGrounded.onGroundedEvent += b =>
        {
            if (isGroundedState == b) return;

            isGroundedState = b;
            stateMachine.ChangeState((b) ? PlayerStateID.Idle : PlayerStateID.InAir);
        };
    }
    private void Update()
    {
        if (stateMachine == null) return;
        //if (UnityLegacy.InputJump() && isGrounded.GetIsGrounded()) Jump();
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
    internal void Jump()
    {
        Jump(Vector3.up);
    }
    internal void Jump(Vector3 direction)
    {
        rigidbody.AddForce(direction * movementData.jumpForce, movementData.jumpForceMode);
        isGrounded.onGroundedEvent?.Invoke(false);
    }
    private void OnDrawGizmos()
    {
        if (stateMachine == null) return;
        stateMachine.OnDrawGizmos();
    }
}
