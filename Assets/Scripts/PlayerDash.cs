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
        rb.AddForce(Vector3.ProjectOnPlane(player.lastMovementDirection, player.ForwardRay().normal) * dashImpulse, ForceMode.Impulse);
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
        stateMachine.ChangeState(StateMachine.StateEnum.Normal);
    }

}
