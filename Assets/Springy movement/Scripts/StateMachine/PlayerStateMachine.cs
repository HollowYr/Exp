using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class PlayerStateMachine
{
    private IPlayerState[] states;
    private PlayerStateAgent agent;
    private PlayerStateID currentState;
    public PlayerStateMachine(PlayerStateAgent agent)
    {
        this.agent = agent;

        int numOfStates = System.Enum.GetNames(typeof(PlayerStateID)).Length;
        states = new IPlayerState[numOfStates];
    }

    public PlayerStateID GetCurrentState() => currentState;

    public void RegisterState(IPlayerState state)
    {
        int index = (int)state.GetID();
        states[index] = state;
    }

    public IPlayerState GetState(PlayerStateID stateID)
    {
        int index = (int)stateID;
        return states[index];
    }

    public void ChangeState(PlayerStateID newState)
    {
        GetState(currentState)?.Exit(agent);
        PlayerStateID previousState = currentState;
        currentState = newState;
        GetState(currentState)?.Enter(agent, previousState);
    }

    public void Update()
    {
        GetState(currentState)?.Update(agent);
    }

    public void FixedUpdate()
    {
        GetState(currentState)?.FixedUpdate(agent);
    }

    public void OnDrawGizmos()
    {
        GetState(currentState)?.OnDrawGizmos();
    }
}
