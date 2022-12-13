using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayerState
{
    PlayerStateID GetID();
    public void Enter(PlayerStateAgent agent);
    public void Update(PlayerStateAgent agent);
    public void Exit(PlayerStateAgent agent);
}