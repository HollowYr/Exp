#define DEBUG

using UnityEngine;

public class PlayerState_Base : IPlayerState
{
    private bool isFirstStart = true;
    protected virtual void Animate(PlayerStateAgent agent) { }

    public virtual void Enter(PlayerStateAgent agent, PlayerStateID previousState)
    {
        if (isFirstStart)
        {
            Init(agent);
            isFirstStart = false;
        }
    }

    protected virtual void Init(PlayerStateAgent agent)
    {
        Debug.Log($"Init: {System.Enum.GetName(typeof(PlayerStateID), GetID())}");
    }

    public virtual void Exit(PlayerStateAgent agent) { }

    public virtual void FixedUpdate(PlayerStateAgent agent) { }

    public virtual PlayerStateID GetID() => throw new System.NotImplementedException();

    public virtual void OnDrawGizmos() { }

    public virtual void Update(PlayerStateAgent agent)
    {
        if (!isFirstStart) Animate(agent);
    }
}

