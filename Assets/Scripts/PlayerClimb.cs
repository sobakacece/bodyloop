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
    Rigidbody rb;
    public override void OnEnter()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        player.hands.GetComponent<RotationConstraint>().enabled = true;
        player.hands.transform.SetParent(null, true);

        player.hands.GetComponent<MeshRenderer>().enabled = true;
    }

    protected override void FixedUpdate()
    {
        player.MyCurrentStamina -= Time.deltaTime * player.staminaReduceSpeed;
        
        player.Climb();

        if (Input.GetKey(KeyCode.LeftShift))
        {
            stateMachine.ChangeState(PlayerStateMachine.StateEnum.Dash);
        }

        if (!Input.GetMouseButton(0) || player.staminaDepleted)
            {
                stateMachine.ChangeState(PlayerStateMachine.StateEnum.Normal);
            }

    }

    public override void OnExit()
    {
        //player.StopCliffMove();
        if (rb == null)
        {
            rb = GetComponent<Rigidbody>();
        }
        rb.useGravity = true;
        player.hands.transform.SetParent(player.cameraHolder, true);
        player.ResetHandsPosition();
        player.hands.GetComponent<MeshRenderer>().enabled = false;
    }

    bool CheckLedge()
    {
        Physics.Raycast(player.hands.transform.position, player.hands.transform.forward, out RaycastHit hit, player.grabDistance, player.climbCollisions);

        Vector3 ledgeCheckOrigin = hit.point + Vector3.up * ledgeOffset;
        return Physics.Raycast(ledgeCheckOrigin, Vector3.down, ledgeCheckDistance);

    }




}
