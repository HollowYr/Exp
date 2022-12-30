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

    public override PlayerStateID GetID() => PlayerStateID.InAir;

    public override void Enter(PlayerStateAgent agent)
    {
        base.Enter(agent);
        playerModel = agent.playerModel;
        distanceToWall = agent.movementData.distanceToWall;
        layerWall = agent.movementData.layerWall;

        Debug.Log($"Enter: {System.Enum.GetName(typeof(PlayerStateID), GetID())}");
    }

    public override void Exit(PlayerStateAgent agent)
    {
        base.Exit(agent);

        Debug.Log($"Exit: {System.Enum.GetName(typeof(PlayerStateID), GetID())}");
    }

    //public override void FixedUpdate(PlayerStateAgent agent)
    //{
    //    base.FixedUpdate(agent);
    //}
    Vector3 wallDestination;
    public override void Update(PlayerStateAgent agent)
    {
        base.Update(agent);
        wallDestination = Vector3.zero;
        Ray rayRight = new Ray(playerModel.position, playerModel.right);
        Ray rayLeft = new Ray(playerModel.position, -playerModel.right);

        Debug.DrawRay(rayLeft.origin, rayLeft.direction, Color.green, Time.deltaTime);
        Debug.DrawRay(rayRight.origin, rayRight.direction, Color.green, Time.deltaTime);

        if (Physics.Raycast(rayRight, out RaycastHit hit, distanceToWall, ~layerWall) ||
            Physics.Raycast(rayLeft, out hit, distanceToWall, ~layerWall))
        {

            //wallDestination = hit.point + (agent.movementData.playerRadius / ((playerModel.position - hit.point).magnitude)) * (playerModel.position - hit.point);
            wallDestination = hit.point + (playerModel.position - hit.point).normalized * agent.movementData.playerRadius;
            wallDestination -= playerModel.localPosition;
            agent.transform.DOMove(wallDestination, 0f).OnComplete(() =>
            agent.stateMachine.ChangeState(PlayerStateID.Wallrun));
            Debug.Log(wallDestination);

            //Debug.Break();
        }
    }

    public override void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(wallDestination, .1f);

    }
}

