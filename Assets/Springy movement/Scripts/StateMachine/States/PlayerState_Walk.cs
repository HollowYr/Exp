using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState_Walk : PlayerState_Base
{
    protected Rigidbody rigidbody;
    private Transform cameraTransform;
    private Transform rotationTransform;
    protected Animator animator;
    private float movementSpeed;
    private float rotationSpeed;
    private float horizontal;
    private float vertical;
    public override PlayerStateID GetID() => PlayerStateID.Walk;

    protected override void Init(PlayerStateAgent agent)
    {
        Debug.Log($"Init: {System.Enum.GetName(typeof(PlayerStateID), GetID())}");
        animator = agent.animator;
        rigidbody = agent.rigidbody;
        cameraTransform = agent.cameraTransform;
        movementSpeed = agent.movementData.movementSpeed;
        rotationTransform = agent.playerModel;
        rotationSpeed = agent.movementData.rotationSpeed;
    }

    public override void Update(PlayerStateAgent agent)
    {
        base.Update(agent);

        horizontal = UnityLegacy.InputHorizontal();
        vertical = UnityLegacy.InputVertical();

        if (UnityLegacy.InputJump() && agent.GetIsGrounded()) agent.Jump();

        if (horizontal == 0 && vertical == 0) agent.stateMachine.ChangeState(PlayerStateID.Idle);
    }
    public override void FixedUpdate(PlayerStateAgent agent) => Move(agent);

    public override void Exit(PlayerStateAgent agent)
    {
        Animate(agent);
        Move(agent);
    }

    private void Move(PlayerStateAgent agent)
    {
        Vector3 movementDirection = agent.GetPlayerMovementDirection();

        rigidbody.velocity = (new Vector3(0, rigidbody.velocity.y, 0) +
                                movementDirection * movementSpeed);

        rotationTransform.RotateInDirectionOnYAxis(movementDirection, rotationSpeed);
    }

    protected override void Animate(PlayerStateAgent agent)
    {
        Vector3 velocity = rigidbody.velocity;
        velocity.y = 0;

        float speed = velocity.magnitude;
        animator.SetFloat("Speed", speed, 0.1f, Time.deltaTime);
    }
}
