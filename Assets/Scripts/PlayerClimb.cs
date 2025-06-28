using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Callbacks;
using UnityEngine;

public class PlayerClimb : PlayerState
{
    // Start is called before the first frame update
    Rigidbody rb;
    public override void OnEnter()
    {
        player.TryGrabCliff();
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
    }

    protected override void FixedUpdate()
    {
        player.currentStamina -= player.currentStamina * Time.deltaTime * player.staminaReduceSpeed;
        player.currentStamina = Math.Clamp(player.currentStamina, 0, player.maxStamina);
        Debug.Log(player.currentStamina);
        rb.velocity = Vector3.zero;


        player.Climb();


        if (!Input.GetMouseButton(0) || !player.CouldGrabSurface() || player.currentStamina <= 0)
        {
            stateMachine.ChangeState(PlayerStateMachine.StateEnum.Normal);
        }
    }

    public override void OnExit()
    {
        player.StopCliffMove();
        rb.useGravity = true;
    }

}
