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
        //Debug.Log($"Enter: {System.Enum.GetName(typeof(PlayerStateID), GetID())}");
    }


    Vector3 wallDestination;
    public override void Update(PlayerStateAgent agent)
    {
        base.Update(agent);

        if (UnityLegacy.InputVertical() <= 0 || previousState == PlayerStateID.Wallrun) return;
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


        //wallDestination = hit.point + (playerModel.position - hit.point).normalized * agent.movementData.playerRadius;
        //wallDestination -= playerModel.localPosition;
        //agent.transform.DOMove(wallDestination, 0f).OnComplete(() => { });
        agent.stateMachine.ChangeState(PlayerStateID.Wallrun);
    }

    public override void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(wallDestination, .1f);

    }
}

