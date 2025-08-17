using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Scripting.APIUpdating;

public class PlayerFall : PlayerState
{
    // Start is called before the first frame update

    public override void OnEnter()
    {
    }
    [SerializeField, Description("delay before entering climb state")]
    private float climbDelayTimer = 0.15f; //added delay, so physics is applied and there is some kind of amplitude
    [SerializeField]
    private float fallMovingSpeed = 3.0f;

    protected override void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
        {
            player.Move(fallMovingSpeed);
        }

        if (player.IsGrounded())
        {
            stateMachine.ChangeState(StateMachine.StateEnum.Normal);
            climbDelayTimer = 0f;
            return;
        }

        if ((player.hands[0].isActive || player.hands[1].isActive) && !player.staminaDepleted)
        {
            if (climbDelayTimer <= 0f)
                climbDelayTimer = Mathf.Clamp(player.rb.velocity.magnitude * 0.1f, 0.05f, 0.15f);

            climbDelayTimer -= Time.fixedDeltaTime;

            if (climbDelayTimer <= 0f)
                stateMachine.ChangeState(StateMachine.StateEnum.Climb);
        }
        else
        {
            climbDelayTimer = 0f; // reset if conditions break
        }
    }

    public override void OnExit()
    {
    }

}
