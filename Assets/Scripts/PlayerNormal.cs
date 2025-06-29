using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerNormal : PlayerState
{
    // Start is called before the first frame update
    public override void OnEnter()
    {
        //player.hands.GetComponent<MeshRenderer>().enabled = false;
    }

    protected override void FixedUpdate()
    {
        player.MyCurrentStamina += Time.deltaTime * player.staminaRecoverySpeed;
        player.MyCurrentStamina = Math.Clamp(player.MyCurrentStamina, 0.0f, player.maxStamina);
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
        {
            player.Move();
        }

        if (player.CouldStartClimb() && Input.GetMouseButton(0) && !player.staminaDepleted)
        {
            stateMachine.ChangeState(PlayerStateMachine.StateEnum.Climb);
        }

    }

}
