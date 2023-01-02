using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayerState
{
    PlayerStateID GetID();
    public void Enter(PlayerStateAgent agent, PlayerStateID previousState);
    public void Update(PlayerStateAgent agent);
    public void FixedUpdate(PlayerStateAgent agent);
    public void Exit(PlayerStateAgent agent);
    public void OnDrawGizmos();
}
