using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR;

public class ClimbingHand : MonoBehaviour
{
    public Transform startTransform;
    public LayerMask climbableMask;
    public bool isActive = true;
    [SerializeField] int buttonIndex = 0;

    [SerializeField]
    private PlayerController player;
    Coroutine cliffCoroutine;


    void Awake()
    {
        startTransform = transform;
    }
    void Start()
    {
        player = transform.root.GetComponent<PlayerController>();

        player.stateMachine.StateEnterEvent += StartClimb;
        player.stateMachine.StateExitEvent += StopClimb;
    }

    void Update()
    {

        if (Input.GetMouseButtonDown(buttonIndex) && player.CouldStartClimb())
        {
            HandActive();
            Debug.Log("Hand active");
        }

        if (Input.GetMouseButtonUp(buttonIndex))
        {
            HandRest();
        }

    }

    public void ResetHandPosition()
    {
        transform.position = startTransform.position;
        transform.rotation = startTransform.rotation;
    }

    void OnDestroy()
    {
        player.stateMachine.StateEnterEvent -= StartClimb;
        player.stateMachine.StateExitEvent -= StopClimb;
    }

    void StartClimb(StateMachine.StateEnum state)
    {
        if (state == StateMachine.StateEnum.Climb && Input.GetMouseButtonDown(buttonIndex))
        {
            HandActive();
        }
    }

    void StopClimb(StateMachine.StateEnum state)
    {
        if (state == StateMachine.StateEnum.Climb)
        {
            HandRest();
        }

    }

    void HandActive()
    {
        transform.SetParent(null, true);
        isActive = true;
        GetComponent<MeshRenderer>().enabled = true;
        if (cliffCoroutine != null)
        {
            StopCoroutine(cliffCoroutine);
        }
        RaycastHit hit = player.ForwardRay();
        Debug.Log("Target point " + hit.point);
        cliffCoroutine = StartCoroutine(MoveHandsToCliff(hit.point));
    }

    void HandRest()
    {
        ResetHandPosition();
        transform.SetParent(player.cameraHolder, true);
        GetComponent<MeshRenderer>().enabled = false;
        isActive = false;
        if (cliffCoroutine != null)
        {
            StopCoroutine(cliffCoroutine);
        }
    }

    public IEnumerator MoveHandsToCliff(Vector3 targetPoint)
    {

        //Quaternion previousRotation = transform.rotation;
        while (true)
        {
            Vector3 direction = (targetPoint - transform.position).normalized;
            float distanceToTarget = Vector3.Distance(transform.position, targetPoint);
            float step = player.magnetSpeed * Time.deltaTime;

            if (distanceToTarget <= 0.05f)
                break;

            transform.position += direction * Mathf.Min(step, distanceToTarget);
            // transform.rotation = Quaternion.Slerp(previousRotation, targetRotation, Time.deltaTime * 5f);
            // previousRotation = transform.rotation;
            yield return null;
        }
        //transform.rotation = targetRotation;
    }


}
