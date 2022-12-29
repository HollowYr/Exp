#define DEBUG
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState_InAir : PlayerState_Walk
{
    public override void Enter(PlayerStateAgent agent)
    {
        base.Enter(agent);
        Debug.Log("Enter: " + System.Enum.GetName(typeof(PlayerStateID), GetID()));
    }

    public override void Exit(PlayerStateAgent agent)
    {
        base.Exit(agent);
        Debug.Log("Exit: " + System.Enum.GetName(typeof(PlayerStateID), GetID()));
    }

    public override void FixedUpdate(PlayerStateAgent agent)
    {
        base.FixedUpdate(agent);
    }

    public override PlayerStateID GetID()
    {
        return PlayerStateID.InAir;
    }

    public override void Update(PlayerStateAgent agent)
    {
        base.Update(agent);
    }
}

