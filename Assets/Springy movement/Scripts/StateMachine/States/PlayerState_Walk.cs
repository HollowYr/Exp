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
    float horizontal;
    float vertical;

    public virtual PlayerStateID GetID()
    {
        return PlayerStateID.Walk;
    }

    public virtual void Enter(PlayerStateAgent agent, PlayerStateID previousState)
    {
        rigidbody = agent.rigidbody;
        cameraTransform = agent.cameraTransform;
        movementSpeed = agent.movementData.movementSpeed;
        rotationTransform = agent.playerModel;
        rotationSpeed = agent.movementData.rotationSpeed;
    }
    public virtual void Update(PlayerStateAgent agent)
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        if (horizontal == 0 && vertical == 0) agent.stateMachine.ChangeState(PlayerStateID.Idle);
    }
    public virtual void FixedUpdate(PlayerStateAgent agent) => Move();

    public virtual void Exit(PlayerStateAgent agent) => Move();

    private void Move()
    {
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


        rotationTransform.RotateInDirectionOnYAxis(movementDirection, rotationSpeed);
    }

    public virtual void OnDrawGizmos() { }
}
