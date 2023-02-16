#define DEBUG
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerState_Wallrun : PlayerState_Base
{
    private Animator animator;
    private Rigidbody rigidbody;
    private Transform playerModel;
    private float distanceToDetectWall;
    private float distanceToWall;
    private float stickToWallPower;
    private float maxTime;
    private float timer;
    private float playerDefaultRadius;
    private float speed;
    private float currentSpeed;
    private float rotationSpeed;
    private float wallMaxAngleDifference;
    private string wallRunAnimation;
    private string isWallOnTheLeftAnimName;
    private Vector3 force;
    private Vector3 rotateToWallDirection;
    private int layerWall;
    private bool isSnapFinished = false;
    private Vector3 previousNormal;

    public override PlayerStateID GetID() => PlayerStateID.Wallrun;

    protected override void Init(PlayerStateAgent agent)
    {
        animator = agent.animator;
        wallRunAnimation = agent.movementData.animationWallRun;
        isWallOnTheLeftAnimName = agent.movementData.isWallOnTheLeft;
        rigidbody = agent.rigidbody;
        playerModel = agent.playerModel;
        playerDefaultRadius = agent.movementData.playerRadius;
        distanceToDetectWall = agent.movementData.distanceToDetectWall;
        layerWall = 1 << agent.movementData.layerWall;
        stickToWallPower = agent.movementData.stickToWallPower;
        maxTime = agent.movementData.maxTime;
        distanceToWall = agent.movementData.distanceToWallOnRun;
        rotationSpeed = agent.movementData.rotationSpeed;
        speed = agent.movementData.wallrunSpeed;
        wallMaxAngleDifference = agent.movementData.wallSnapMaxAngle;
    }

    public override void Enter(PlayerStateAgent agent, PlayerStateID previousState)
    {
        base.Enter(agent, previousState);
        timer = 0;

        LockPositionConstraintY();

        Vector3 wallDestination = Vector3.zero;
        Ray rayRight = new Ray(playerModel.position, playerModel.right);
        Ray rayLeft = new Ray(playerModel.position, -playerModel.right);
        Ray diagonalRight = new Ray(playerModel.position, playerModel.right + playerModel.forward);
        Ray diagonalLeft = new Ray(playerModel.position, -playerModel.right + playerModel.forward);

        if (!Physics.Raycast(rayRight, out RaycastHit hit, distanceToDetectWall, layerWall) &&
            !Physics.Raycast(rayLeft, out hit, distanceToDetectWall, layerWall) &&
            !Physics.Raycast(diagonalLeft, out hit, distanceToDetectWall, layerWall) &&
            !Physics.Raycast(diagonalRight, out hit, distanceToDetectWall, layerWall))
            return;

        agent.ModifyColliderRadius(playerDefaultRadius + distanceToWall);
        bool isWallOnTheLeft;
        if (Vector3.Dot(playerModel.right.normalized, hit.normal.normalized) > 0)
            isWallOnTheLeft = true;
        else
            isWallOnTheLeft = false;
        animator.SetBool(isWallOnTheLeftAnimName, isWallOnTheLeft);

        previousNormal = hit.normal;
        wallDestination = hit.point + (playerModel.position - hit.point).normalized * agent.movementData.playerRadius;
        // compensate difference in Y position
        wallDestination -= playerModel.localPosition;
        agent.transform.DOMove(wallDestination, agent.movementData.wallSnapTime)
            .OnComplete(() => isSnapFinished = true);

        agent.movementData.WallrunChangeFOV(agent.cmVCamera);

        currentSpeed = rigidbody.velocity.magnitude;
        DOTween.To(() => currentSpeed, x => currentSpeed = x, speed, .5f);
    }

    public override void Update(PlayerStateAgent agent)
    {
        base.Update(agent);
        if (!isSnapFinished) return;

        if (UnityLegacy.InputJump())
        {
            agent.Jump();
            ChangeStateToInAir(agent);
            return;
        }

        Ray rayRight = new Ray(playerModel.position, playerModel.right);
        Ray rayLeft = new Ray(playerModel.position, -playerModel.right);
        Ray diagonalRight = new Ray(playerModel.position, playerModel.right + playerModel.forward);
        Ray diagonalLeft = new Ray(playerModel.position, -playerModel.right + playerModel.forward);

        if (Physics.Raycast(rayRight, out RaycastHit hit, distanceToDetectWall, layerWall) ||
            Physics.Raycast(rayLeft, out hit, distanceToDetectWall, layerWall) ||
            Physics.Raycast(diagonalRight, out hit, distanceToDetectWall, layerWall) ||
            Physics.Raycast(diagonalLeft, out hit, distanceToDetectWall, layerWall))
        {
            force = (hit.point - (hit.point + hit.normal)) * stickToWallPower * Time.fixedDeltaTime;
            timer += Time.deltaTime;
            rotateToWallDirection = Vector3.Cross(hit.normal, Vector3.up);

            if (Vector3.Angle(previousNormal, hit.normal) > wallMaxAngleDifference)
                ChangeStateToInAir(agent);

            previousNormal = hit.normal;
        }
        else
        {
            ChangeStateToInAir(agent);
        }
    }


    public override void FixedUpdate(PlayerStateAgent agent)
    {
        if (!isSnapFinished) return;

        if (timer > maxTime || UnityLegacy.InputVertical() <= 0)
        {
            ChangeStateToInAir(agent);
            return;
        }

        if (rotateToWallDirection != Vector3.zero)
        {
            if ((playerModel.forward - rotateToWallDirection).magnitude > (playerModel.forward - -rotateToWallDirection).magnitude)
                rotateToWallDirection = -rotateToWallDirection;
            playerModel.RotateInDirectionOnYAxis(rotateToWallDirection, rotationSpeed);
        }
        float input = Mathf.CeilToInt(UnityLegacy.InputVertical());
        rigidbody.velocity = force * input + playerModel.forward * input * (currentSpeed);
        rotateToWallDirection = Vector3.zero;
    }

    protected override void Animate(PlayerStateAgent agent)
    {
        animator.Play(wallRunAnimation);
    }

    private static void ChangeStateToInAir(PlayerStateAgent agent) =>
        agent.stateMachine.ChangeState(PlayerStateID.InAir);

    public override void Exit(PlayerStateAgent agent)
    {
        agent.ModifyColliderRadius(playerDefaultRadius);
        ResetConstraints();
        agent.movementData.ResetFOV(agent.cmVCamera);
    }
    private void LockPositionConstraintY()
    {
        rigidbody.constraints = RigidbodyConstraints.FreezeRotationX |
                                RigidbodyConstraints.FreezeRotationY |
                                RigidbodyConstraints.FreezeRotationZ |
                                RigidbodyConstraints.FreezePositionY;
    }

    private void ResetConstraints()
    {
        rigidbody.constraints = RigidbodyConstraints.FreezeRotationX |
                                RigidbodyConstraints.FreezeRotationY |
                                RigidbodyConstraints.FreezeRotationZ;
    }

    public override void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Ray rayRight = new Ray(playerModel.position, playerModel.right);
        Ray rayLeft = new Ray(playerModel.position, -playerModel.right);
        Ray diagonalRight = new Ray(playerModel.position, playerModel.right + playerModel.forward);
        Ray diagonalLeft = new Ray(playerModel.position, -playerModel.right + playerModel.forward);

        Gizmos.DrawRay(rayRight);
        Gizmos.DrawRay(rayLeft);
        Gizmos.DrawRay(diagonalRight);
        Gizmos.DrawRay(diagonalLeft);
        Gizmos.DrawRay(rigidbody.position, rotateToWallDirection);
        Gizmos.DrawSphere(rigidbody.position, .1f);
    }
}