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
    private Vector3 force;
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

        timer = 0;

        LockPositionConstraintY();
        Debug.Log($"Enter: {System.Enum.GetName(typeof(PlayerStateID), GetID())}");
    }

    public void Update(PlayerStateAgent agent)
    {
        Ray rayRight = new Ray(playerModel.position, playerModel.right);
        Ray rayLeft = new Ray(playerModel.position, -playerModel.right);

        if (Physics.Raycast(rayRight, out RaycastHit hit, distanceToWall, ~layerWall) ||
            Physics.Raycast(rayLeft, out hit, distanceToWall, ~layerWall))
        {
            force = (hit.point - (hit.point + hit.normal)) * stickToWallPower * Time.fixedDeltaTime;
            timer += Time.deltaTime;
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

        Vector2 input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).normalized;
        rigidbody.velocity += force * input.x * input.y + playerModel.forward * input.y * additionalSpeed;

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

    }
}

