#define DEBUG
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerState_Wallrun : IPlayerState
{
    private Rigidbody rigidbody;
    private Transform playerModel;
    private float distanceToWall;
    private float stickToWallPower;
    private float maxTime;
    private float timer;
    private float speed;
    private float additionalSpeed;
    private float rotationSpeed;
    private float wallMaxAngleDifference;
    private Vector3 force;
    private Vector3 rotateToWallDirection;
    private int layerWall;
    private bool isSnapFinished = false;

    private Vector3 previousNormal;

    public PlayerStateID GetID() => PlayerStateID.Wallrun;

    public void Enter(PlayerStateAgent agent, PlayerStateID previousState)
    {
        rigidbody = agent.rigidbody;
        playerModel = agent.playerModel;
        distanceToWall = agent.movementData.distanceToWall;
        layerWall = 1 << agent.movementData.layerWall;
        stickToWallPower = agent.movementData.stickToWallPower;
        maxTime = agent.movementData.maxTime;
        additionalSpeed = agent.movementData.additionalSpeed;
        rotationSpeed = agent.movementData.rotationSpeed;
        speed = agent.movementData.movementSpeed;
        wallMaxAngleDifference = agent.movementData.wallSnapMaxAngle;
        timer = 0;

        LockPositionConstraintY();

        Vector3 wallDestination = Vector3.zero;
        Ray rayRight = new Ray(playerModel.position, playerModel.right);
        Ray rayLeft = new Ray(playerModel.position, -playerModel.right);

        if (!Physics.Raycast(rayRight, out RaycastHit hit, distanceToWall, layerWall) &&
            !Physics.Raycast(rayLeft, out hit, distanceToWall, layerWall))
            return;

        previousNormal = hit.normal;
        wallDestination = hit.point + (playerModel.position - hit.point).normalized * agent.movementData.playerRadius;
        wallDestination -= playerModel.localPosition;
        agent.transform.DOMove(wallDestination, agent.movementData.wallSnapTime)
            .OnComplete(() => isSnapFinished = true);
    }

    public void Update(PlayerStateAgent agent)
    {
        if (!isSnapFinished) return;
        Ray rayRight = new Ray(playerModel.position, playerModel.right);
        Ray rayLeft = new Ray(playerModel.position, -playerModel.right);
        Ray diagonalRight = new Ray(playerModel.position, playerModel.right + playerModel.forward);
        Ray diagonalLeft = new Ray(playerModel.position, -playerModel.right + playerModel.forward);

        if (Physics.Raycast(rayRight, out RaycastHit hit, distanceToWall, layerWall) ||
            Physics.Raycast(rayLeft, out hit, distanceToWall, layerWall) ||
            Physics.Raycast(diagonalRight, out hit, distanceToWall, layerWall) ||
            Physics.Raycast(diagonalLeft, out hit, distanceToWall, layerWall))
        {
            force = (hit.point - (hit.point + hit.normal)) * stickToWallPower * Time.fixedDeltaTime;
            timer += Time.deltaTime;
            rotateToWallDirection = Vector3.Cross(hit.normal, Vector3.up);

            if (Vector3.Angle(previousNormal, hit.normal) > wallMaxAngleDifference)
                agent.stateMachine.ChangeState(PlayerStateID.InAir);

            previousNormal = hit.normal;
        }
        else
        {
            agent.stateMachine.ChangeState(PlayerStateID.InAir);
        }
    }

    public void FixedUpdate(PlayerStateAgent agent)
    {
        if (!isSnapFinished) return;

        if (timer > maxTime || UnityLegacy.InputVertical() <= 0)
        {
            agent.stateMachine.ChangeState(PlayerStateID.InAir);
            return;
        }

        if (rotateToWallDirection != Vector3.zero)
        {
            if ((playerModel.forward - rotateToWallDirection).magnitude > (playerModel.forward - -rotateToWallDirection).magnitude)
                rotateToWallDirection = -rotateToWallDirection;
            playerModel.RotateInDirectionOnYAxis(rotateToWallDirection, rotationSpeed * 10f);
        }
        float input = Mathf.CeilToInt(UnityLegacy.InputVertical());
        rigidbody.velocity = force * input + playerModel.forward * input * (speed + additionalSpeed);
        rotateToWallDirection = Vector3.zero;
    }

    public void Exit(PlayerStateAgent agent) => ResetConstraints();

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

    public void OnDrawGizmos()
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

