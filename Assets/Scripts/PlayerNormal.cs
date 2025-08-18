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
    [SerializeField]
    private float coyotteTime = 0.25f;
    Coroutine coyotteTimeRoutine;
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

        if (player.CouldStartClimb() && (player.hands[0].isActive || player.hands[1].isActive) && !player.staminaDepleted)
        {
            stateMachine.ChangeState(StateMachine.StateEnum.Climb);
        }

        if (!player.IsGrounded())
        {
            coyotteTimeRoutine = StartCoroutine(Coyotte(coyotteTime));
        }

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            player.Jump(player.jumpHeight);
    }

    public IEnumerator Coyotte(float sec)
    {
        yield return new WaitForSeconds(sec);
        stateMachine.ChangeState(StateMachine.StateEnum.Fall);
        //transform.rotation = targetRotation;
    }

    public override void OnExit()
    {
        base.OnExit();
        if (coyotteTimeRoutine != null)
        {
            StopCoroutine(coyotteTimeRoutine);
        }
    }

}
