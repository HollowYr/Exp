#define DEBUG
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
    private PathCreator pathCreator;
    private float playerRadius;
    private int layerRails;
    private float speed;
    private float railsOffset;
    private float playerHeight;
    public override PlayerStateID GetID() => PlayerStateID.RailGrind;

    protected override void Init(PlayerStateAgent agent)
    {
        Debug.Log($"Init: {System.Enum.GetName(typeof(PlayerStateID), GetID())}");
        MovementData movementData = agent.movementData;
        animator = agent.animator;
        layerRails = 1 << movementData.layerRails;
        playerRadius = agent.movementData.playerRadius;
        playerModel = agent.playerModel;
        animationName = agent.movementData.animationRailGrinding;
        speed = movementData.railsMovementSpeed;
        rotationTransform = agent.playerModel;
        playerHeight = movementData.playerDesiredFloatHeight;
        railsOffset = movementData.railsMovementOffset + movementData.playerDesiredFloatHeight;
    }

    public override void Enter(PlayerStateAgent agent, PlayerStateID previousState)
    {
        base.Enter(agent, previousState);
        animator.Play(animationName);

        agent.allowGroundedStateChange = false;
        if (Physics.SphereCast(playerModel.position, playerRadius, -playerModel.up, out RaycastHit hit, playerHeight + .5f, layerRails))
        {
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
        }
    }

    private EndOfPathInstruction endOfPathInstruction = EndOfPathInstruction.Stop;
    private float distanceTravelled;

    public override void Update(PlayerStateAgent agent)
    {
        base.Update(agent);
        if (pathCreator == null) return;

        distanceTravelled += speed * Time.deltaTime;
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
    }
}