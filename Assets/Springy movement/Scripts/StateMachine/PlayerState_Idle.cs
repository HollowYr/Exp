using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState_Idle : IPlayerState
{
    public PlayerStateID GetID()
    {
        return PlayerStateID.Idle;
    }

    public void Enter(PlayerStateAgent agent)
    {

        Debug.Log("Enter: " + System.Enum.GetName(typeof(PlayerStateID), GetID()));
    }
    public void Update(PlayerStateAgent agent)
    {

    }

    public void Exit(PlayerStateAgent agent)
    {
        Debug.Log("Exit: " + System.Enum.GetName(typeof(PlayerStateID), GetID()));

    }
}
