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
    void Start()
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

    void Update()
    {
        if (stateMachine == null) return;
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded.GetIsGrounded()) Jump();
        stateMachine.Update();
    }

    private void Jump()
    {
        rigidbody.AddForce(Vector3.up * movementData.jumpForce, movementData.jumpForceMode);
        isGrounded.onGroundedEvent?.Invoke(false);
    }

    private void FixedUpdate()
    {
        if (stateMachine == null) return;
        stateMachine.FixedUpdate();
    }

    private void OnDrawGizmos()
    {
        if (stateMachine == null) return;
        stateMachine.OnDrawGizmos();
    }
}
