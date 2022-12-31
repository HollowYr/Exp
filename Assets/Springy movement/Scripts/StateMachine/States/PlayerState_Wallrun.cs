#define DEBUG
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerState_Wallrun : IPlayerState
{
    private Rigidbody rigidbody;
    private Transform playerModel;
    private Transform playerTransform;
    private float distanceToWall;
    private float stickToWallPower;
    private float maxTime;
    private float timer;
    private float additionalSpeed;
    private float rotationSpeed;
    private Vector3 force;
    private Vector3 rotateToWallDirection;
    private int layerWall;

    public PlayerStateID GetID() => PlayerStateID.Wallrun;

    public void Enter(PlayerStateAgent agent)
    {
        rigidbody = agent.rigidbody;
        playerModel = agent.playerModel;
        distanceToWall = agent.movementData.distanceToWall;
        layerWall = agent.movementData.layerWall;
        playerTransform = agent.transform;
        stickToWallPower = agent.movementData.stickToWallPower;
        maxTime = agent.movementData.maxTime;
        additionalSpeed = agent.movementData.additionalSpeed;
        rotationSpeed = agent.movementData.rotationSpeed;

        timer = 0;

        LockPositionConstraintY();
        Debug.Log($"Enter: {System.Enum.GetName(typeof(PlayerStateID), GetID())}");
    }

    public void Update(PlayerStateAgent agent)
    {
        Ray rayRight = new Ray(playerModel.position, playerModel.right);
        Ray rayLeft = new Ray(playerModel.position, -playerModel.right);
        Ray diagonalRight = new Ray(playerModel.position, playerModel.right + playerModel.forward);
        Ray diagonalLeft = new Ray(playerModel.position, -playerModel.right + playerModel.forward);

        if (Physics.Raycast(rayRight, out RaycastHit hit, distanceToWall, ~layerWall) ||
            Physics.Raycast(rayLeft, out hit, distanceToWall, ~layerWall) ||
            Physics.Raycast(diagonalRight, out hit, distanceToWall, ~layerWall) ||
            Physics.Raycast(diagonalLeft, out hit, distanceToWall, ~layerWall))
        {
            force = (hit.point - (hit.point + hit.normal)) * stickToWallPower * Time.fixedDeltaTime;
            timer += Time.deltaTime;
            rotateToWallDirection = Vector3.Cross(hit.normal, Vector3.up);
        }
        else
        {
            agent.stateMachine.ChangeState(PlayerStateID.InAir);
        }
    }

    public void FixedUpdate(PlayerStateAgent agent)
    {
        if (timer > maxTime || Input.GetAxis("Vertical") <= 0)
        {
            ResetConstraints();
            agent.stateMachine.ChangeState(PlayerStateID.InAir);
            return;
        }

        if (rotateToWallDirection != Vector3.zero)
        {
            if ((playerModel.forward - rotateToWallDirection).magnitude > (playerModel.forward - -rotateToWallDirection).magnitude)
                rotateToWallDirection = -rotateToWallDirection;
            playerModel.RotateInDirectionOnYAxis(rotateToWallDirection, rotationSpeed * 10f);
            //Debug.Break();
        }
        Vector2 input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).normalized;
        rigidbody.velocity += force * input.y + playerModel.right * input.x + playerModel.forward * input.y * additionalSpeed;
        rotateToWallDirection = Vector3.zero;
    }

    public void Exit(PlayerStateAgent agent)
    {
        ResetConstraints();
        Debug.Log($"Exit: {System.Enum.GetName(typeof(PlayerStateID), GetID())}");
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
        rigidbody.constraints = RigidbodyConstraints.None;
        rigidbody.constraints =
                                RigidbodyConstraints.FreezeRotationX |
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

