using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class PlayerDash : PlayerState
{

    public float dashImpulse = 10f;
    public float dashTime = 0.5f;

    public float staminaUsage = 15;
    Rigidbody rb;
    public override void OnEnter()
    {
        rb = player.GetComponent<Rigidbody>();
        Physics.Raycast(player.transform.position, player.transform.forward, out RaycastHit hit, 2.0f, player.climbCollisions);
        rb.AddForce(Vector3.ProjectOnPlane(player.lastMovementDirection, hit.normal) * dashImpulse, ForceMode.Impulse);
        rb.useGravity = false;
        player.MyCurrentStamina -= staminaUsage;
        StartCoroutine(ReturnToNormal());
    }
    protected override void FixedUpdate()
    {
        //stateMachine.ChangeState(PlayerStateMachine.StateEnum.Normal);
        
    }

    public override void OnExit()
    {
        rb.useGravity = true;
    }

    private IEnumerator ReturnToNormal()
    {
        yield return new WaitForSeconds(0.3f);
        stateMachine.ChangeState(PlayerStateMachine.StateEnum.Normal);
    }

}
