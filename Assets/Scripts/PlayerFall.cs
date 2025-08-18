using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Scripting.APIUpdating;

public class PlayerFall : PlayerState
{
    // Start is called before the first frame update


    [SerializeField, Description("delay before entering climb state")]
    private float climbDelayTimer = 0.15f; //added delay, so physics is applied and there is some kind of amplitude

    private float currentClimbTimer;
    [SerializeField]
    private float fallMovingSpeed = 3.0f;

    protected override void FixedUpdate()
    {
        if ((player.hands[0].isActive || player.hands[1].isActive) && !player.staminaDepleted)
        {
            stateMachine.ChangeState(StateMachine.StateEnum.Climb);
            return;
        }

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
        {
            player.Move(fallMovingSpeed);
        }

        if (player.IsGrounded() && (!player.hands[0].isActive || player.hands[1].isActive))
        {
            stateMachine.ChangeState(StateMachine.StateEnum.Normal);
        }


    }

    public override void OnExit()
    {
    }

    public override void OnEnter()
    {
        currentClimbTimer = climbDelayTimer;
    }


}
