using System;
using System.Collections;

using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float playerCameraFOV = 60;
    [SerializeField] private float baseSpeed;
    [SerializeField] private float rotationSpeedY, rotationSpeedX;
    [SerializeField] private float jumpHeight;
    [SerializeField] public float grabDistance;

    [SerializeField] public Transform headCamera;
    [SerializeField] public Transform cameraHolder;
    [SerializeField] private Rigidbody rb;
    //   [SerializeField] private SimpleGroundChecker groundChecker;
    [SerializeField] private Collider mainCollider;

    public PlayerStateMachine stateMachine;

    [SerializeField] public LayerMask climbCollisions;
    [SerializeField] public GameObject hands;


    private Camera viewCamera;
    private float speed;
    public float mouseScale = 1.0f;
    public float BaseSpeed => baseSpeed;
    public float CurrentSpeed => speed;

    [SerializeField] private float climbingSpeed = 10.0f;
    [SerializeField] public float maxStamina = 100.0f;
    [SerializeField]
    public float MyCurrentStamina
    {
        get => currentStamina;
        set
        {
            currentStamina = Math.Clamp(value, 0, maxStamina);
            if (currentStamina <= 0)
            {
                staminaDepleted = true;
            }
            else if (currentStamina > 10)
            {
                staminaDepleted = false;
            }
            staminaProgress.ImageProgress = currentStamina / maxStamina;
        }
    }
    private float currentStamina;
    public bool staminaDepleted = false;
    [SerializeField] public float staminaReduceSpeed = 10.0f;
    [SerializeField] public float staminaRecoverySpeed = 20.0f;

    [SerializeField] public float handMaxDistance = 1.0f;


    public Vector3 spawnPoint;
    public Quaternion spawnRotation;

    public Vector3 handsLocalSpawnPosition;
    public Quaternion handsLocalSpawnRotation;

    [SerializeField] private RadialMenu staminaProgress;


    public Vector3 lastMovementDirection;
    private Vector3 lastPreviousPosition;

    void Awake()
    {
        spawnPoint = transform.position;
        spawnRotation = transform.rotation;

        handsLocalSpawnPosition = hands.transform.localPosition;
        handsLocalSpawnRotation = hands.transform.localRotation;

    }
    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        MyCurrentStamina = maxStamina;
        speed = baseSpeed;
        viewCamera = headCamera.GetComponent<Camera>();
        stateMachine = GetComponent<PlayerStateMachine>();

    }


    public bool IsGrounded()
    {
        return Physics.Raycast(transform.position, -Vector3.up, mainCollider.bounds.size.y);
    }

    public bool CouldStartClimb()
    {
        return Physics.Raycast(headCamera.position, headCamera.forward, grabDistance, climbCollisions);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded())
            Jump(jumpHeight);

        if (Input.GetKeyDown(KeyCode.R))
            Restart();

        // Debug.Log(IsGrounded());
        if (Application.isFocused)
            Look();

        Vector3 currentPosition = transform.position;
        Vector3 actualMovement = currentPosition - lastPreviousPosition;
        if (actualMovement.magnitude > 0.01f)
        {
        lastMovementDirection = actualMovement.normalized;

        }

        lastPreviousPosition = currentPosition;
    }

    public void SetCurrentSpeed(float newValue)
    {
        speed = newValue;
        HandleSpeedChange(newValue);
    }

    private void HandleSpeedChange(float newValue)
    {
        float multiplier = (CurrentSpeed / baseSpeed);
        viewCamera.fieldOfView = playerCameraFOV * multiplier;
    }
    private void Restart()
    {
        GameFlow.Instance.GameRestart();
    }

    public void Move()
    {
        Vector2 input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        if (input == Vector2.zero) return;

        Vector3 delta = (cameraHolder.forward * input.y + cameraHolder.right * input.x).normalized * speed * Time.deltaTime;

        Vector3 rayOrigin = transform.position;
        Vector3 rayDirection = delta.normalized;
        float rayLength = delta.magnitude + mainCollider.bounds.extents.x;

        Debug.DrawRay(rayOrigin, rayDirection * rayLength, Color.red);

        if (Physics.Raycast(rayOrigin, rayDirection, out RaycastHit hit, rayLength, Physics.AllLayers, QueryTriggerInteraction.Ignore))
        {
            Vector3 surfaceForward = Vector3.Cross(hit.normal, Vector3.up).normalized;
            if (Vector3.Dot(surfaceForward, rayDirection) < 0)
                surfaceForward = -surfaceForward;

            float angle = Vector3.Angle(rayDirection, hit.normal);
            float frictionImitaion = Mathf.InverseLerp(160f, 120f, angle);

            delta = surfaceForward * delta.magnitude * frictionImitaion;
        }

        rb.MovePosition(transform.position + new Vector3(delta.x, 0f, delta.z));
    }


    private void Look()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseScale;
        float mouseY = -Input.GetAxis("Mouse Y") * UnityEngine.Time.deltaTime * rotationSpeedY * mouseScale;
        //Debug.Log($"{nameof(mouseX)} {mouseX}; {nameof(mouseY)} {mouseY};");
        cameraHolder.localEulerAngles += new Vector3(0, mouseX, 0) * UnityEngine.Time.deltaTime * rotationSpeedY;

        Quaternion q = Quaternion.Euler(mouseY, 0, 0) * headCamera.localRotation;
        Quaternion handsQ = Quaternion.Euler(mouseY, 0, 0) * headCamera.localRotation;
        //cameraAngle = Vector3.Angle(cameraHolder.forward, q.eulerAngles);

        if (Mathf.Abs(q.x) <= 0.7)
            headCamera.localRotation = q;
        if (stateMachine.currentState != PlayerStateMachine.StateEnum.Climb)
        {

            hands.transform.rotation = handsQ;
            Vector3 handsOffset = headCamera.TransformPoint(new Vector3(handsLocalSpawnPosition.x, -handsLocalSpawnPosition.y, handsLocalSpawnPosition.z));
            hands.transform.position = handsOffset;
        }
    }

    private void Jump(float height)
    {
        stateMachine.ChangeState(PlayerStateMachine.StateEnum.Normal);
        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        rb.AddForce(Vector3.up * height, ForceMode.Impulse);
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(30, 30, 360, 180), $"CurrentState: {stateMachine.currentState}");
    }

    public void GrabCliff()
    {
        // Vector3 origin = headCamera.position + headCamera.forward * 0.5f;
        // Vector3 forward = headCamera.forward;

        // // const int rayCount = 5;
        // // const float angleStep = 10f;
        // const float rayDistance = 1f;


        // if (Physics.Raycast(origin, forward, out RaycastHit hit, rayDistance, climbCollisions))
        // {
        //     //transform.position = hit.point - transform.forward * 0.5f;
        //     if (cliffCoroutine != null)
        //         StopCoroutine(cliffCoroutine);

        //     cliffCoroutine = StartCoroutine(MoveToCliff(hit.point, Quaternion.LookRotation(-hit.normal, Vector3.up)));
        // }
    }

    // private IEnumerator MoveToCliff(Vector3 targetPoint, Quaternion targetRotation)
    // {
    //     rb.velocity = Vector3.zero;
    //     Quaternion previousRotation = transform.rotation;
    //     while (true)
    //     {
    //         Vector3 direction = (targetPoint - transform.position).normalized;
    //         float distanceToTarget = Vector3.Distance(transform.position, targetPoint);
    //         float step = magnetSpeed * Time.deltaTime;

    //         isCliffCoroutineRunning = true;
    //         if (distanceToTarget <= 1.5f)
    //             break;

    //         if (Physics.Raycast(transform.position, direction, step + 0.1f, Physics.AllLayers))
    //             break;

    //         transform.position += direction * Mathf.Min(step, distanceToTarget);
    //         // transform.rotation = Quaternion.Slerp(previousRotation, targetRotation, Time.deltaTime * 5f);
    //         // previousRotation = transform.rotation;
    //         yield return null;
    //     }
    //     isCliffCoroutineRunning = false;
    //     //transform.rotation = targetRotation;
    // }

    // public void StopCliffMove()
    // {
    //     if (cliffCoroutine != null)
    //     {
    //         StopCoroutine(cliffCoroutine);
    //     }
    // }

    public void Climb(float forceModifier = 1, ForceMode forceMode = ForceMode.VelocityChange)
    {
        rb.velocity = Vector3.zero;
        Vector3 surfaceNormal = UpdateHandsPosition();

        Vector2 input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        if (input == Vector2.zero) return;

        Vector3 camForward = Vector3.ProjectOnPlane(headCamera.forward, surfaceNormal).normalized;
        if (camForward.sqrMagnitude < 0.01f)
        {

            camForward = Vector3.ProjectOnPlane(headCamera.right, surfaceNormal);
        }

        camForward.Normalize();
        Vector3 camRight = Vector3.ProjectOnPlane(headCamera.right, surfaceNormal).normalized;

        Vector3 moveDir = camRight * input.x + camForward * input.y;

        rb.AddForce(moveDir.normalized * climbingSpeed * forceModifier, forceMode);

        Vector3 toHands = hands.transform.position - transform.position;


        if (toHands.magnitude > handMaxDistance)
        {
            Vector3 clampedPosition = hands.transform.position - toHands.normalized * handMaxDistance;
            rb.position = clampedPosition;
        }
    }



    Vector3 UpdateHandsPosition()
    {
        Vector3 origin = headCamera.position + headCamera.forward * 0.5f;
        Vector3 direction = headCamera.forward;

        const float rayDistance = 2f;
        const float lerpSpeed = 10f;

        if (Physics.Raycast(origin, direction, out RaycastHit hit, rayDistance, climbCollisions))
        {
            Vector3 targetPosition = hit.point + hit.normal * -0.01f;

            Vector3 offsetFromBody = targetPosition - transform.position;
            if (offsetFromBody.sqrMagnitude > grabDistance * grabDistance)
            {
                offsetFromBody = offsetFromBody.normalized * grabDistance;
                targetPosition = transform.position + offsetFromBody;
            }

            hands.transform.position = Vector3.Lerp(hands.transform.position, targetPosition, Time.deltaTime * lerpSpeed);

            hands.transform.rotation = Quaternion.LookRotation(-hit.normal);
            return hit.normal;
        }

        return Vector3.zero;
    }

    public void ClimbLedge()
    {
        // Debug.Log("Ledge");
    }

    public void ResetHandsPosition()
    {
        hands.transform.localPosition = handsLocalSpawnPosition;
        hands.transform.localRotation = handsLocalSpawnRotation;
    }

    public void Respawn()
    {
        transform.position = spawnPoint;
        transform.rotation = spawnRotation;
        currentStamina = maxStamina;

    }

}