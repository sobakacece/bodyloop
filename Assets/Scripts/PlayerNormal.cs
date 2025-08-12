using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerNormal : PlayerState
{
    // Start is called before the first frame update
    [SerializeField]
    private float movingSpeed = 5.0f;
    public override void OnEnter()
    {
        //player.hands.GetComponent<MeshRenderer>().enabled = false;
    }

    protected override void FixedUpdate()
    {
        player.MyCurrentStamina += Time.deltaTime * player.staminaRecoverySpeed;
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
        {
            player.Move(movingSpeed);
        }

        if (player.CouldStartClimb() && (Input.GetMouseButton(0) || Input.GetMouseButton(1)) && !player.staminaDepleted)
        {
            stateMachine.ChangeState(StateMachine.StateEnum.Climb);
        }

        if (!player.IsGrounded())
        {
            stateMachine.ChangeState(StateMachine.StateEnum.Fall);

        }

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && player.IsGrounded())
            player.Jump(player.jumpHeight);
    }

}
