#define DEBUG
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerState_InAir : PlayerState_Walk
{
    private Transform playerModel;
    private float distanceToWall;
    private float delayToNextWall;
    private bool delayPassed = false;
    private int layerWall;
    private PlayerStateID previousState;

    public override PlayerStateID GetID() => PlayerStateID.InAir;

    public override void Enter(PlayerStateAgent agent, PlayerStateID previousState)
    {
        base.Enter(agent, previousState);
        this.previousState = previousState;
        playerModel = agent.playerModel;
        distanceToWall = agent.movementData.distanceToWall;
        layerWall = 1 << agent.movementData.layerWall;
        delayToNextWall = agent.movementData.delayToNextWall;
        delayPassed = false;
        if (previousState == PlayerStateID.Wallrun)
        {
            agent.DoAfterTime(delayToNextWall, () => delayPassed = true);
        }
        else
        {
            delayPassed = true;
        }
        //Debug.Log($"Enter: {System.Enum.GetName(typeof(PlayerStateID), GetID())}");
    }

    Vector3 wallDestination;
    public override void Update(PlayerStateAgent agent)
    {
        base.Update(agent);

        if (UnityLegacy.InputVertical() <= 0 || !delayPassed) return;
        wallDestination = Vector3.zero;
        Ray rayRight = new Ray(playerModel.position, playerModel.right);
        Ray rayLeft = new Ray(playerModel.position, -playerModel.right);

#if DEBUG
        Debug.DrawRay(rayLeft.origin, rayLeft.direction, Color.green, Time.deltaTime);
        Debug.DrawRay(rayRight.origin, rayRight.direction, Color.green, Time.deltaTime);
#endif
        if (!Physics.Raycast(rayRight, out RaycastHit hit, distanceToWall, layerWall) &&
            !Physics.Raycast(rayLeft, out hit, distanceToWall, layerWall))
            return;

        agent.stateMachine.ChangeState(PlayerStateID.Wallrun);
    }

    public override void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(wallDestination, .1f);
    }
}

