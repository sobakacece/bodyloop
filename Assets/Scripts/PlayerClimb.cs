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

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            player.Jump(player.jumpHeight);
            stateMachine.ChangeState(StateMachine.StateEnum.Fall);
        }
    }

    protected override void FixedUpdate()
    {
        player.MyCurrentStamina -= Time.deltaTime * player.staminaReduceSpeed;

        player.Climb();



        // if (Input.GetKey(KeyCode.LeftShift))
        // {
        //     stateMachine.ChangeState(PlayerStateMachine.StateEnum.Dash);
        // }

        if ((!Input.GetMouseButton(0) && !Input.GetMouseButton(1)) || player.staminaDepleted)
        {
            if (player.IsGrounded())
            {
                stateMachine.ChangeState(StateMachine.StateEnum.Normal);

            }
            else
            {

                stateMachine.ChangeState(StateMachine.StateEnum.Fall);
            }
        }
    }

    public override void OnExit()
    {

        if (rb == null)
        {
            rb = GetComponent<Rigidbody>();
        }
        rb.useGravity = true;
        player.lastMagnetPosition = Vector3.zero;

    }

    // bool CheckLedge()
    // {
    //     Physics.Raycast(player.leftHand.transform.position, player.leftHand.transform.forward, out RaycastHit hit, player.grabDistance, player.climbCollisions);

    //     Vector3 ledgeCheckOrigin = hit.point + Vector3.up * ledgeOffset;
    //     return Physics.Raycast(ledgeCheckOrigin, Vector3.down, ledgeCheckDistance);

    // }




}
