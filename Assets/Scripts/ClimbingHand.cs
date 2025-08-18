using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR;

public class ClimbingHand : MonoBehaviour
{
    public Transform startTransform;
    public LayerMask climbableMask;
    [SerializeField] public float magnetSpeed = 7.0f;

    public bool isActive = true;
    [SerializeField] int buttonIndex = 0;

    [SerializeField]
    private PlayerController player;
    Coroutine cliffCoroutine;

    private bool wasReseted;


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
        if (Input.GetMouseButtonUp(buttonIndex))
        {
            wasReseted = true;
        }

        if (Input.GetMouseButton(buttonIndex) && player.CouldStartClimb() && player.stateMachine.currentState == StateMachine.StateEnum.Fall && wasReseted)
        {
            HandActive();
            return;
        }

        if (Input.GetMouseButtonDown(buttonIndex) && player.CouldStartClimb())
        {
            HandActive();
            return;
        }


        if (Input.GetMouseButtonUp(buttonIndex))
        {
            HandRest();
            return;
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
        wasReseted = false;
        transform.SetParent(null, true);
        isActive = true;
        GetComponent<MeshRenderer>().enabled = true;
        if (cliffCoroutine != null)
        {
            StopCoroutine(cliffCoroutine);
        }
        // Debug.Log("Target point " + hit.point);
        cliffCoroutine = StartCoroutine(MoveHandsToCliff(player.ForwardRay().point));
    }

    void HandRest()
    {
        isActive = false;
        ResetHandPosition();
        transform.SetParent(player.cameraHolder, true);
        GetComponent<MeshRenderer>().enabled = false;
        if (cliffCoroutine != null)
        {
            StopCoroutine(cliffCoroutine);
        }
    }

    public IEnumerator MoveHandsToCliff(Vector3 targetPoint)
    {
        Debug.Log("Moved to Cliff");
        //Quaternion previousRotation = transform.rotation;
        while (true)
        {
            Vector3 direction = (targetPoint - transform.position).normalized;
            float distanceToTarget = Vector3.Distance(transform.position, targetPoint);
            float step = magnetSpeed * Time.deltaTime;

            if (distanceToTarget <= 0.01f)
                break;

            transform.position += direction * Mathf.Min(step, distanceToTarget);
            // transform.rotation = Quaternion.Slerp(previousRotation, targetRotation, Time.deltaTime * 5f);
            // previousRotation = transform.rotation;
            yield return null;
        }
        //transform.rotation = targetRotation;
    }


}
