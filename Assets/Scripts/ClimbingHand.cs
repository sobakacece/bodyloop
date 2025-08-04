using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbingHand : MonoBehaviour
{
    public Transform startTransform;
    public LayerMask climbableMask;
    private bool isActive = true;
    [SerializeField] int buttonIndex = 0;

    [SerializeField]
    private PlayerController player;
    Coroutine cliffCoroutine;

    bool isCliffCoroutineRunning;

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

    void FixedUpdate()
    {
        isActive = Input.GetMouseButton(buttonIndex);
    }

    public void ResetHandPosition()
    {
        gameObject.transform.position = startTransform.position;
        gameObject.transform.rotation = startTransform.rotation;
    }

    void OnDestroy()
    {
        player.stateMachine.StateEnterEvent -= StartClimb;
        player.stateMachine.StateExitEvent -= StopClimb;
    }

    void StartClimb(PlayerStateMachine.StateEnum state)
    {
        if (state == PlayerStateMachine.StateEnum.Climb)
        {
            transform.SetParent(null, true);

            GetComponent<MeshRenderer>().enabled = true;
            if (cliffCoroutine != null)
            {
                StopCoroutine(cliffCoroutine);
            }
            RaycastHit hit = player.ForwardRay();
            cliffCoroutine = StartCoroutine(MoveHandsToCliff(hit.point));

        }
    }

    void StopClimb(PlayerStateMachine.StateEnum state)
    {
        if (state == PlayerStateMachine.StateEnum.Climb)
        {
            player.leftHand.transform.SetParent(player.cameraHolder, true);
            player.leftHand.ResetHandPosition();
            player.leftHand.GetComponent<MeshRenderer>().enabled = false;

            if (cliffCoroutine != null)
            {
                StopCoroutine(cliffCoroutine);
            }
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

            isCliffCoroutineRunning = true;
            if (distanceToTarget <= 0.2f)
                break;

            if (Physics.Raycast(transform.position, direction, step + 0.1f, Physics.AllLayers))
                break;

            transform.position += direction * Mathf.Min(step, distanceToTarget);
            // transform.rotation = Quaternion.Slerp(previousRotation, targetRotation, Time.deltaTime * 5f);
            // previousRotation = transform.rotation;
            yield return null;
        }
        isCliffCoroutineRunning = false;
        //transform.rotation = targetRotation;
    }


}
