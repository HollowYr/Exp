#define DEBUG
using DG.Tweening;
using PathCreation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState_RailGrind : PlayerState_Base
{
    private string animationName;
    private Animator animator;
    private Transform rotationTransform;
    private Transform playerModel;
    private Rigidbody rigidbody;
    private PathCreator pathCreator;
    private float playerRadius;
    private int layerRails;
    private float speed;
    private float currentSpeed;
    private float railsOffset;
    private float playerHeight;
    private EndOfPathInstruction endOfPathInstruction = EndOfPathInstruction.Stop;
    private float distanceTravelled;

    public override PlayerStateID GetID() => PlayerStateID.RailGrind;
    protected override void Init(PlayerStateAgent agent)
    {
        base.Init(agent);
        MovementData movementData = agent.movementData;
        animator = agent.animator;
        layerRails = 1 << movementData.layerRails;
        playerRadius = agent.movementData.playerRadius;
        playerModel = agent.playerModel;
        animationName = agent.movementData.animationRailGrinding;
        speed = movementData.railsMovementSpeed;
        rigidbody = agent.rigidbody;
        rotationTransform = agent.playerModel;
        playerHeight = movementData.playerDesiredFloatHeight;
        railsOffset = movementData.railsMovementOffset + movementData.playerDesiredFloatHeight;
    }

    public override void Enter(PlayerStateAgent agent, PlayerStateID previousState)
    {
        base.Enter(agent, previousState);
        animator.Play(animationName);

        agent.allowGroundedStateChange = false;
        Ray rayDown = new Ray(playerModel.position, -playerModel.up);
        if (!Physics.SphereCast(rayDown, playerRadius, out RaycastHit hit, playerHeight + .5f, layerRails))
            return;

        pathCreator = hit.transform.GetComponentInChildren<PathCreator>();
        distanceTravelled = pathCreator.path.GetClosestDistanceAlongPath(agent.transform.position);

        // check is reversed moving needed
        Vector3 currentPosition = pathCreator.path.GetPointAtDistance(distanceTravelled, endOfPathInstruction);
        Vector3 nextPositionAlongPath = pathCreator.path.GetPointAtDistance(distanceTravelled + 2 * speed * Time.deltaTime, endOfPathInstruction);
        Vector3 direction = nextPositionAlongPath - currentPosition;
        if (Vector3.Dot(direction.normalized, playerModel.forward.normalized) < 0)
        {
            speed = -speed;
        }

        currentSpeed = rigidbody.velocity.magnitude;
        DOTween.To(() => currentSpeed, x => currentSpeed = x, speed, .5f);

        agent.movementData.RailGrindChangeFOV(agent.cmVCamera);
    }



    public override void Update(PlayerStateAgent agent)
    {
        base.Update(agent);
        if (pathCreator == null) return;

        distanceTravelled += currentSpeed * Time.deltaTime;
        if (pathCreator.path.length < distanceTravelled || distanceTravelled < 0)
        {
            ChangeStateToInAir(agent);
            return;
        }

        if (UnityLegacy.InputJump())
        {
            agent.Jump();
            ChangeStateToInAir(agent);
            return;
        }
        Vector3 position = pathCreator.path.GetPointAtDistance(distanceTravelled, endOfPathInstruction);
        position.y += railsOffset;
        Vector3 rotationPosition = pathCreator.path.GetPointAtDistance(distanceTravelled + speed * Time.deltaTime, endOfPathInstruction);
        rotationTransform.RotateInDirectionOnYAxis(rotationPosition - position, 360f);
        agent.transform.position = position;
    }

    private static void ChangeStateToInAir(PlayerStateAgent agent) =>
            agent.stateMachine.ChangeState(PlayerStateID.InAir);
    public override void Exit(PlayerStateAgent agent)
    {
        agent.DoAfterTime(0.13f, () => agent.allowGroundedStateChange = true);
        speed = Mathf.Abs(speed);
        agent.rigidbody.velocity += playerModel.forward * speed;

        agent.movementData.ResetFOV(agent.cmVCamera);
    }
}