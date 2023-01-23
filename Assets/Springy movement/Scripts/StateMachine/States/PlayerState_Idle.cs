using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState_Idle : IPlayerState
{
    public PlayerStateID GetID() => PlayerStateID.Idle;
    public void Enter(PlayerStateAgent agent, PlayerStateID previousState) { }
    public void Update(PlayerStateAgent agent)
    {
        float horizontal = Mathf.Abs(Input.GetAxis("Horizontal"));
        float vertical = Mathf.Abs(Input.GetAxis("Vertical"));

        if (UnityLegacy.InputJump() && agent.GetIsGrounded()) agent.Jump();

        if (horizontal > 0 || vertical > 0) agent.stateMachine.ChangeState(PlayerStateID.Walk);
    }
    public void FixedUpdate(PlayerStateAgent agent) { }
    public void Exit(PlayerStateAgent agent) { }
    public void OnDrawGizmos() { }
}
