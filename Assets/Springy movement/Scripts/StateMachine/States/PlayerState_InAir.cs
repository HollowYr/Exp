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
    // private Animator animator;
    private MovementData movementData;
    // private Rigidbody rigidbody;
    PlayerStateID previousState;
    private float delayToChangeState;
    private float distanceToWall;
    private float distanceToRail;
    private int layerWall;
    private int layerRails;
    private bool canChangeState = false;
    private bool canPlayJumpingDown = true;
    public override PlayerStateID GetID() => PlayerStateID.InAir;

    protected override void Init(PlayerStateAgent agent)
    {
        base.Init(agent);
        Debug.Log($"Init: {System.Enum.GetName(typeof(PlayerStateID), GetID())}");
        // rigidbody = agent.rigidbody;
        movementData = agent.movementData;
        playerModel = agent.playerModel;
        distanceToWall = movementData.distanceToDetectWall;
        layerWall = 1 << movementData.layerWall;
        layerRails = 1 << movementData.layerRails;
        delayToChangeState = movementData.delayToNextWall;
        distanceToRail = movementData.playerDesiredFloatHeight;
        animator = agent.animator;
    }

    public override void Enter(PlayerStateAgent agent, PlayerStateID previousState)
    {
        base.Enter(agent, previousState);

        this.previousState = previousState;
        animator.Play(movementData.jumpingUp);

        canPlayJumpingDown = true;
        canChangeState = false;
        if (previousState == PlayerStateID.Wallrun ||
            previousState == PlayerStateID.RailGrind)
        {
            agent.DoAfterTime(delayToChangeState, () => canChangeState = true);
        }
        else
        {
            canChangeState = true;
        }
        //Debug.Log($"Enter: {System.Enum.GetName(typeof(PlayerStateID), GetID())}");
    }

    public override void Update(PlayerStateAgent agent)
    {
        base.Update(agent);

        WallrunCheck();

        RailGrindCheck();


        void WallrunCheck()
        {
            if (UnityLegacy.InputVertical() <= 0 || (!canChangeState && previousState == PlayerStateID.Wallrun)) return;
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
            Debug.DrawRay(rayLeft.origin, rayLeft.direction, Color.green, Time.deltaTime);
            Debug.DrawRay(rayRight.origin, rayRight.direction, Color.green, Time.deltaTime);
            Debug.DrawRay(diagonalLeft.origin, diagonalLeft.direction, Color.green, Time.deltaTime);
            Debug.DrawRay(diagonalRight.origin, diagonalRight.direction, Color.green, Time.deltaTime);
#endif
        }

        void RailGrindCheck()
        {
            if (!canChangeState && previousState == PlayerStateID.RailGrind) return;
            Ray rayDown = new Ray(playerModel.position, -playerModel.up);
            if (Physics.Raycast(rayDown, out RaycastHit hit, distanceToRail + .5f, layerRails))
                agent.stateMachine.ChangeState(PlayerStateID.RailGrind);

#if DEBUG && UNITY_EDITOR
            Debug.DrawRay(rayDown.origin, rayDown.direction, Color.green, Time.deltaTime);
#endif
        }
    }

    protected override void Animate(PlayerStateAgent agent)
    {
        if (rigidbody.velocity.y >= 0 || !canPlayJumpingDown) return;

        animator.Play(movementData.animationJumpingDown);
        canPlayJumpingDown = false;
    }
}