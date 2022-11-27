using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateAgent : MonoBehaviour
{
    public PlayerStateMachine stateMachine;
    public PlayerStateID initialState;
    Rigidbody rb;

    void Start()
    {
        stateMachine = new PlayerStateMachine(this);
        stateMachine.RegisterState(new PlayerState_Idle());
        stateMachine.RegisterState(new PlayerState_Walk());
        stateMachine.ChangeState(initialState);

        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (rb.velocity.magnitude > 10f)
        {
            stateMachine.ChangeState(PlayerStateID.Walk);
        }
        else if (rb.velocity.magnitude < 1f)
        {
            stateMachine.ChangeState(PlayerStateID.Idle);
        }
        if (stateMachine != null)
            stateMachine.Update();
    }
}
