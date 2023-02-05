#define DEBUG
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerState_InAir : PlayerState_Walk
{
    private Transform playerModel;
    private PlayerStateID previousState;
    private float delayToChangeState;
    private float delayForWallrunState;
    private float delayForRailsState;
    private float distanceToWall;
    private float distanceToRail;
    private float playerRadius;
    private string jumpingUp;
    private string jumpingDown;
    private int layerWall;
    private int layerRails;
    private bool canChangeState = false;
    private bool canPlayJumpingDown = true;
    public override PlayerStateID GetID() => PlayerStateID.InAir;

    protected override void Init(PlayerStateAgent agent)
    {
        base.Init(agent);
        Debug.Log($"Init: {System.Enum.GetName(typeof(PlayerStateID), GetID())}");
        MovementData movementData = agent.movementData;
        playerModel = agent.playerModel;
        playerRadius = movementData.playerRadius;
        distanceToWall = movementData.distanceToDetectWall;
        layerWall = 1 << movementData.layerWall;
        layerRails = 1 << movementData.layerRails;
        delayForWallrunState = movementData.delayForNextWall;
        delayForRailsState = movementData.delayForRailsState;
        distanceToRail = movementData.playerDesiredFloatHeight;
        jumpingUp = movementData.jumpingUp;
        jumpingDown = movementData.animationJumpingDown;
        animator = agent.animator;
    }

    public override void Enter(PlayerStateAgent agent, PlayerStateID previousState)
    {
        base.Enter(agent, previousState);

        this.previousState = previousState;
        animator.Play(jumpingUp);

        canPlayJumpingDown = true;
        canChangeState = false;
        delayToChangeState = GetDelayByState(previousState);
        if (delayToChangeState > 0)
        {
            agent.DoAfterTime(delayToChangeState, () => canChangeState = true);
        }
        else
        {
            canChangeState = true;
        }
    }

    private float GetDelayByState(PlayerStateID previousState)
    {
        float delay = 0;
        switch (previousState)
        {
            case PlayerStateID.Wallrun:
                delay = delayForWallrunState;
                break;

            case PlayerStateID.RailGrind:
                delay = delayForRailsState;
                break;
        }
        return delay;
    }

    public override void Update(PlayerStateAgent agent)
    {
        base.Update(agent);

        WallrunCheck();
        RailGrindCheck();

        void WallrunCheck()
        {
            if (UnityLegacy.InputVertical() <= 0 || !canChangeState && previousState == PlayerStateID.Wallrun) return;

            Ray rayRight = new Ray(playerModel.position, playerModel.right);
            Ray rayLeft = new Ray(playerModel.position, -playerModel.right);
            Ray diagonalRight = new Ray(playerModel.position, playerModel.right + playerModel.forward);
            Ray diagonalLeft = new Ray(playerModel.position, -playerModel.right + playerModel.forward);

            if (Physics.Raycast(rayRight, out RaycastHit hit, distanceToWall, layerWall) ||
                Physics.Raycast(rayLeft, out hit, distanceToWall, layerWall) ||
                Physics.Raycast(diagonalRight, out hit, distanceToWall, layerWall) ||
                Physics.Raycast(diagonalLeft, out hit, distanceToWall, layerWall))
                agent.stateMachine.ChangeState(PlayerStateID.Wallrun);

#if DEBUG && UNITY_EDITOR
            Debug.DrawRay(rayLeft.origin, rayLeft.direction * distanceToWall, Color.green, Time.deltaTime);
            Debug.DrawRay(rayRight.origin, rayRight.direction * distanceToWall, Color.green, Time.deltaTime);
            Debug.DrawRay(diagonalLeft.origin, diagonalLeft.direction * distanceToWall, Color.green, Time.deltaTime);
            Debug.DrawRay(diagonalRight.origin, diagonalRight.direction * distanceToWall, Color.green, Time.deltaTime);
#endif
        }

        void RailGrindCheck()
        {
            if ((!canChangeState && previousState == PlayerStateID.RailGrind) ||
                !Physics.SphereCast(playerModel.position, playerRadius, -playerModel.up,
                 out RaycastHit hit, distanceToRail + .5f, layerRails))
                return;

            agent.stateMachine.ChangeState(PlayerStateID.RailGrind);
        }
    }

    protected override void Animate(PlayerStateAgent agent)
    {
        if (rigidbody.velocity.y >= 0 || !canPlayJumpingDown) return;

        animator.Play(jumpingDown);
        canPlayJumpingDown = false;
    }
}