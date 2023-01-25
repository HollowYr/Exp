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
    private float distanceToRail;
    private bool delayPassed = false;
    private int layerWall;
    private int layerRails;
    private PlayerStateID previousState;
    public override PlayerStateID GetID() => PlayerStateID.InAir;
    private bool isFirstStart = true;
    public override void Enter(PlayerStateAgent agent, PlayerStateID previousState)
    {
        base.Enter(agent, previousState);
        this.previousState = previousState;
        delayPassed = false;

        if (isFirstStart)
        {
            playerModel = agent.playerModel;
            distanceToWall = agent.movementData.distanceToWall;
            layerWall = 1 << agent.movementData.layerWall;
            layerRails = 1 << agent.movementData.layerRails;
            delayToNextWall = agent.movementData.delayToNextWall;
            distanceToRail = agent.movementData.playerDesiredFloatHeight;
            isFirstStart = false;
        }


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

    public override void Update(PlayerStateAgent agent)
    {
        base.Update(agent);

        if (UnityLegacy.InputVertical() <= 0 || !delayPassed) return;
        Ray rayRight = new Ray(playerModel.position, playerModel.right);
        Ray rayLeft = new Ray(playerModel.position, -playerModel.right);
        Ray rayDown = new Ray(playerModel.position, -playerModel.up);
        Ray diagonalRight = new Ray(playerModel.position, playerModel.right + playerModel.forward);
        Ray diagonalLeft = new Ray(playerModel.position, -playerModel.right + playerModel.forward);

#if DEBUG && UNITY_EDITOR
        Debug.DrawRay(rayLeft.origin, rayLeft.direction, Color.green, Time.deltaTime);
        Debug.DrawRay(rayRight.origin, rayRight.direction, Color.green, Time.deltaTime);
        Debug.DrawRay(diagonalLeft.origin, diagonalLeft.direction, Color.green, Time.deltaTime);
        Debug.DrawRay(diagonalRight.origin, diagonalRight.direction, Color.green, Time.deltaTime);
        Debug.DrawRay(rayDown.origin, rayDown.direction, Color.green, Time.deltaTime);
#endif
        if (Physics.Raycast(rayRight, out RaycastHit hit, distanceToWall, layerWall) ||
            Physics.Raycast(rayLeft, out hit, distanceToWall, layerWall) ||
            Physics.Raycast(diagonalRight, out hit, distanceToWall, layerWall) ||
            Physics.Raycast(diagonalLeft, out hit, distanceToWall, layerWall))
            agent.stateMachine.ChangeState(PlayerStateID.Wallrun);

        if (Physics.Raycast(rayDown, out hit, distanceToRail + .5f, layerRails))
            agent.stateMachine.ChangeState(PlayerStateID.RailGrind);

    }

    public override void OnDrawGizmos() { }
}

