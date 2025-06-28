using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerNormal : PlayerState
{
    // Start is called before the first frame update


    protected override void FixedUpdate()
    {
        player.currentStamina += player.currentStamina * Time.deltaTime * player.staminaRecoverySpeed;
        player.currentStamina = Math.Clamp(player.currentStamina, 0.0f, player.maxStamina);
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
        {
            player.Move();
        }

        if (player.CouldGrabSurface() && Input.GetMouseButton(0))
        {
            stateMachine.ChangeState(PlayerStateMachine.StateEnum.Climb);
        }

    }

}
