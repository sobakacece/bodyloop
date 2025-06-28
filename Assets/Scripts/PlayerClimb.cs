using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.Animations;

public class PlayerClimb : PlayerState
{
    // Start is called before the first frame update

    public float ledgeOffset = 2.0f;
    public float ledgeCheckDistance = 3.0f;
    Rigidbody rb;
    public override void OnEnter()
    {
        player.GrabCliff();
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        player.hands.GetComponent<RotationConstraint>().enabled = true;
        player.hands.transform.SetParent(null, true); 
    }

    protected override void FixedUpdate()
    {
        player.currentStamina -= player.currentStamina * Time.deltaTime * player.staminaReduceSpeed;
        player.currentStamina = Math.Clamp(player.currentStamina, 0, player.maxStamina);



        if (!player.CouldStartClimb() && CheckLedge())
        {
            player.ClimbLedge();
        }
        else
        {
            player.Climb();
        }

        if (!Input.GetMouseButton(0) || player.currentStamina <= 0)
        {
            stateMachine.ChangeState(PlayerStateMachine.StateEnum.Normal);
        }
    }

    public override void OnExit()
    {
        player.StopCliffMove();
        if (rb == null)
        {
            rb = GetComponent<Rigidbody>();
        }
        rb.useGravity = true;
        player.hands.GetComponent<RotationConstraint>().enabled = false;
        player.hands.transform.SetParent(player.cameraHolder, true);
        player.ResetHandsPosition();
    }

    bool CheckLedge()
    {
        Physics.Raycast(player.headCamera.transform.position, player.headCamera.transform.forward, out RaycastHit hit, player.grabDistance, player.climbCollisions);

        Vector3 ledgeCheckOrigin = hit.point + Vector3.up * ledgeOffset;
        return Physics.Raycast(ledgeCheckOrigin, Vector3.down, out RaycastHit downHit, ledgeCheckDistance);

    }


}
