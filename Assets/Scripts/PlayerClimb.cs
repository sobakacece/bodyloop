using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class PlayerClimb : PlayerState
{
    // Start is called before the first frame update

    public float ledgeOffset = 0.3f;
    public float ledgeCheckDistance = 3.0f;

    Coroutine cliffCoroutine;
    Rigidbody rb;
    public override void OnEnter()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.velocity = Vector3.zero;
        // player.hands.GetComponent<RotationConstraint>().enabled = true;

    }

    protected override void FixedUpdate()
    {
        player.MyCurrentStamina -= Time.deltaTime * player.staminaReduceSpeed;

        player.Climb();

        // if (Input.GetKey(KeyCode.LeftShift))
        // {
        //     stateMachine.ChangeState(PlayerStateMachine.StateEnum.Dash);
        // }

        if (!Input.GetMouseButton(0) || player.staminaDepleted)
        {
            stateMachine.ChangeState(PlayerStateMachine.StateEnum.Normal);
        }
    }

    public override void OnExit()
    {
 
        if (rb == null)
        {
            rb = GetComponent<Rigidbody>();
        }
        rb.useGravity = true;
        
    }

    // bool CheckLedge()
    // {
    //     Physics.Raycast(player.leftHand.transform.position, player.leftHand.transform.forward, out RaycastHit hit, player.grabDistance, player.climbCollisions);

    //     Vector3 ledgeCheckOrigin = hit.point + Vector3.up * ledgeOffset;
    //     return Physics.Raycast(ledgeCheckOrigin, Vector3.down, ledgeCheckDistance);

    // }




}
