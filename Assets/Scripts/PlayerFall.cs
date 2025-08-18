using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Scripting.APIUpdating;

public class PlayerFall : PlayerState
{
    // Start is called before the first frame update



    [SerializeField]
    private float fallMovingSpeed = 3.0f;

    protected override void FixedUpdate()
    {


        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
        {
            player.Move(fallMovingSpeed);
        }

        if (player.IsGrounded() && !player.hands[0].isActive && !player.hands[1].isActive)
        {
            stateMachine.ChangeState(StateMachine.StateEnum.Normal);
        }


    }

    void Update()
    {
        if ((player.hands[0].isActive || player.hands[1].isActive) && !player.staminaDepleted)
        {
            stateMachine.ChangeState(StateMachine.StateEnum.Climb);
            return;
        }
    }

    public override void OnExit()
    {
    }

    public override void OnEnter()
    {
    }


}
