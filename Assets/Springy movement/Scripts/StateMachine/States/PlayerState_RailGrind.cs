#define DEBUG
using PathCreation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState_RailGrind : IPlayerState
{
    private Transform rotationTransform;
    Transform playerModel;
    private PathCreator pathCreator;
    int layerRails;
    private float railsOffset;
    private float rotationSpeed;
    private float playerHeight;
    private bool isFirstStart = true;
    public PlayerStateID GetID() => PlayerStateID.RailGrind;
    public void Enter(PlayerStateAgent agent, PlayerStateID previousState)
    {
        if (isFirstStart)
        {
            MovementData movementData = agent.movementData;
            layerRails = 1 << movementData.layerRails;
            playerModel = agent.playerModel;
            rotationTransform = agent.playerModel;
            rotationSpeed = movementData.rotationSpeed;
            playerHeight = movementData.playerDesiredFloatHeight;
            railsOffset = movementData.railsMovementOffset + movementData.playerDesiredFloatHeight;
            isFirstStart = false;
        }

        agent.allowGroundedStateChange = false;
        Ray ray = new Ray(playerModel.position, -playerModel.up);
        if (Physics.Raycast(ray, out RaycastHit hit, playerHeight + .5f, layerRails))
        {
            pathCreator = hit.transform.GetComponentInChildren<PathCreator>();
            distanceTravelled = pathCreator.path.GetClosestDistanceAlongPath(agent.transform.position);
        }
    }

    public EndOfPathInstruction endOfPathInstruction = EndOfPathInstruction.Stop;
    public float speed = 5;
    private float distanceTravelled;

    public void Update(PlayerStateAgent agent)
    {
        if (pathCreator == null) return;

        distanceTravelled += speed * Time.deltaTime;
        if (pathCreator.path.length < distanceTravelled)
        {
            agent.stateMachine.ChangeState(PlayerStateID.InAir);
            return;
        }

        Vector3 position = pathCreator.path.GetPointAtDistance(distanceTravelled, endOfPathInstruction);
        position.y += railsOffset;
        //agent.transform.rotation = pathCreator.path.GetRotationAtDistance(distanceTravelled, endOfPathInstruction);
        Vector3 rotationPosition = pathCreator.path.GetPointAtDistance(distanceTravelled + speed * Time.deltaTime, endOfPathInstruction);
        rotationTransform.RotateInDirectionOnYAxis(rotationPosition - position, 360f);
        agent.transform.position = position;
    }

    public void FixedUpdate(PlayerStateAgent agent) { }

    public void Exit(PlayerStateAgent agent)
    {

        agent.allowGroundedStateChange = true;
        agent.rigidbody.velocity += playerModel.forward * speed;
    }

    public void OnDrawGizmos() { }
}

