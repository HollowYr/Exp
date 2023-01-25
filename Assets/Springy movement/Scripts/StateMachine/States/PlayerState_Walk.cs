using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState_Walk : IPlayerState
{
    private Rigidbody rigidbody;
    private Transform cameraTransform;
    private Transform rotationTransform;
    private float movementSpeed;
    private float rotationSpeed;
    private float horizontal;
    private float vertical;
    private bool isFirstStart = true;
    public virtual PlayerStateID GetID() => PlayerStateID.Walk;
    public virtual void Enter(PlayerStateAgent agent, PlayerStateID previousState)
    {
        if (!isFirstStart) return;

        rigidbody = agent.rigidbody;
        cameraTransform = agent.cameraTransform;
        movementSpeed = agent.movementData.movementSpeed;
        rotationTransform = agent.playerModel;
        rotationSpeed = agent.movementData.rotationSpeed;
        isFirstStart = false;
    }
    public virtual void Update(PlayerStateAgent agent)
    {
        horizontal = UnityLegacy.InputHorizontal();
        vertical = UnityLegacy.InputVertical();

        if (UnityLegacy.InputJump() && agent.GetIsGrounded()) agent.Jump();

        if (horizontal == 0 && vertical == 0) agent.stateMachine.ChangeState(PlayerStateID.Idle);
    }
    public virtual void FixedUpdate(PlayerStateAgent agent) => Move(agent);

    public virtual void Exit(PlayerStateAgent agent) => Move(agent);

    private void Move(PlayerStateAgent agent)
    {
        Vector3 movementDirection = agent.GetPlayerMovementDirection();

        rigidbody.velocity = (new Vector3(0, rigidbody.velocity.y, 0) +
                                movementDirection * movementSpeed);

        rotationTransform.RotateInDirectionOnYAxis(movementDirection, rotationSpeed);
    }

    public virtual void OnDrawGizmos() { }
}
