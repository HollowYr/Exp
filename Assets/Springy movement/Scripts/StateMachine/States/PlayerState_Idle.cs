using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PlayerState_Idle : PlayerState_Base
{
    private Animator animator;
    private Rigidbody rigidbody;
    public override PlayerStateID GetID() => PlayerStateID.Idle;

    protected override void Init(PlayerStateAgent agent)
    {
        animator = agent.animator;
        rigidbody = agent.rigidbody;
    }

    public override void Enter(PlayerStateAgent agent, PlayerStateID previousState)
    {
        base.Enter(agent, previousState);
        animator.Play(agent.movementData.movementTree);
    }

    public override void Update(PlayerStateAgent agent)
    {
        base.Update(agent);
        float horizontal = Mathf.Abs(Input.GetAxis("Horizontal"));
        float vertical = Mathf.Abs(Input.GetAxis("Vertical"));
        rigidbody.velocity = animator.transform.forward * vertical + animator.transform.right * horizontal + Vector3.up * rigidbody.velocity.y;

        if (UnityLegacy.InputJump() && agent.GetIsGrounded()) agent.Jump();

        if (horizontal > 0 || vertical > 0) agent.stateMachine.ChangeState(PlayerStateID.Walk);
    }
    protected override void Animate(PlayerStateAgent agent)
    {
        Vector3 velocity = rigidbody.velocity;
        velocity.y = 0;

        float speed = velocity.magnitude;
        animator.SetFloat("Speed", speed, 0.1f, Time.deltaTime);
    }
}
