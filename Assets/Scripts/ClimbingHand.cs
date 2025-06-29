using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbingHand : MonoBehaviour
{
    public Transform handL;
    public Transform handR;
    public float regripDistance = 0.4f;
    public float reachSpeed = 10f;
    public LayerMask climbableMask;

    private Transform activeHand;
    private Transform staticHand;
    private Vector3 activeTargetPos;
    private Quaternion activeTargetRot;
    private bool isLeftActive = true;
    private Vector3 lastClimberPosition;

    [SerializeField]
    private PlayerController player;

    void Start()
    {
        activeHand = handL;
        staticHand = handR;
        lastClimberPosition = transform.position;
    }

    void Update()
    {
        if (player.stateMachine.currentState == PlayerStateMachine.StateEnum.Climb)
        {
        Vector3 climbDelta = transform.position - lastClimberPosition;

        if (climbDelta.magnitude >= regripDistance)
        {
            SwapHands();
            FindNewGripPoint();
            lastClimberPosition = transform.position;
        }

        activeHand.position = Vector3.Lerp(activeHand.position, activeTargetPos, Time.deltaTime * reachSpeed);
        activeHand.rotation = Quaternion.Slerp(activeHand.rotation, activeTargetRot, Time.deltaTime * reachSpeed);
        }
        
    }

    void SwapHands()
    {
        isLeftActive = !isLeftActive;
        (activeHand, staticHand) = (staticHand, activeHand);
    }

    void FindNewGripPoint()
    {
        Vector3 origin = staticHand.position + transform.forward * 0.2f;
        Vector3 dir = transform.forward;

        if (Physics.Raycast(origin, dir, out RaycastHit hit, 1f, climbableMask))
        {
            Vector3 offset = hit.normal * 0.05f;
            activeTargetPos = hit.point + offset;
            activeTargetRot = Quaternion.LookRotation(-hit.normal, Vector3.up);
        }
        else
        {
            activeTargetPos = activeHand.position; 
            activeTargetRot = activeHand.rotation;
        }
    }
}
