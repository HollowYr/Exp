#define DEBUG
using PathCreation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState_RailGrind : PlayerState_Base
{
    private Animator animator;
    private Transform rotationTransform;
    private Transform playerModel;
    private PathCreator pathCreator;
    int layerRails;
    private float speed;
    private float railsOffset;
    private float rotationSpeed;
    private float playerHeight;
    public override PlayerStateID GetID() => PlayerStateID.RailGrind;

    protected override void Init(PlayerStateAgent agent)
    {
        Debug.Log($"Init: {System.Enum.GetName(typeof(PlayerStateID), GetID())}");
        MovementData movementData = agent.movementData;
        animator = agent.animator;
        layerRails = 1 << movementData.layerRails;
        playerModel = agent.playerModel;
        speed = movementData.railsMovementSpeed;
        rotationTransform = agent.playerModel;
        rotationSpeed = movementData.rotationSpeed;
        playerHeight = movementData.playerDesiredFloatHeight;
        railsOffset = movementData.railsMovementOffset + movementData.playerDesiredFloatHeight;
    }

    public override void Enter(PlayerStateAgent agent, PlayerStateID previousState)
    {
        base.Enter(agent, previousState);
        animator.Play("RailGrinding");

        agent.allowGroundedStateChange = false;
        Ray rayDown = new Ray(playerModel.position, -playerModel.up);
        if (Physics.Raycast(rayDown, out RaycastHit hit, playerHeight + .5f, layerRails))
        {
            pathCreator = hit.transform.GetComponentInChildren<PathCreator>();
            distanceTravelled = pathCreator.path.GetClosestDistanceAlongPath(agent.transform.position);
        }
    }

    public EndOfPathInstruction endOfPathInstruction = EndOfPathInstruction.Stop;
    private float distanceTravelled;

    public override void Update(PlayerStateAgent agent)
    {
        base.Update(agent);
        if (pathCreator == null) return;

        distanceTravelled += speed * Time.deltaTime;
        if (pathCreator.path.length < distanceTravelled)
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
        agent.allowGroundedStateChange = true;
        agent.rigidbody.velocity += playerModel.forward * speed;
    }
}

