using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float playerCameraFOV = 60;
    [SerializeField] private float baseSpeed;
    [SerializeField] private float rotationSpeedY, rotationSpeedX;
    [SerializeField] private float jumpHeight;
    [SerializeField] public float grabDistance = 2.0f;

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
    public bool isCliffCoroutineRunning = false;
    [SerializeField] public float staminaReduceSpeed = 10.0f;
    [SerializeField] public float staminaRecoverySpeed = 20.0f;

    [SerializeField] public float handMaxDistance = 0.7f;
    [SerializeField] public float handDistanceOffset = 0.3f;
    [SerializeField] public float magnetSpeed = 2.0f;


    public Vector3 spawnPoint;
    public Quaternion spawnRotation;

    public Vector3 handsLocalSpawnPosition;
    public Quaternion handsLocalSpawnRotation;

    [SerializeField] private GameObject hudPrefab;
    [SerializeField] private RadialMenu staminaProgress;


    public Vector3 lastMovementDirection;
    private Vector3 lastPreviousPosition;
    public bool inZone;
    [SerializeField]
    private Text hintText;

    void Awake()
    {
        spawnPoint = transform.position;
        spawnRotation = transform.rotation;

        handsLocalSpawnPosition = hands.transform.localPosition;
        handsLocalSpawnRotation = hands.transform.localRotation;

    }
    private void Start()
    {
        GameObject hudInstance = Instantiate(hudPrefab);
        //.transform.SetParent(hands.transform);
        staminaProgress = hudInstance.GetComponent<RadialMenu>();


        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        MyCurrentStamina = maxStamina;
        speed = baseSpeed;
        viewCamera = headCamera.GetComponent<Camera>();
        stateMachine = GetComponent<PlayerStateMachine>();
        GameFlow.Instance.sensetivityChanged += (float value) =>
        {
            mouseScale = value;
        };
        mouseScale = GameFlow.Instance.baseSens;

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
        //        Debug.Log(hands.transform.forward);
        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded())
            Jump(jumpHeight);

        if (Input.GetKeyDown(KeyCode.R))
            Restart();

        // Debug.Log(IsGrounded());
        if (Application.isFocused)
            Look();

        if (Input.GetKeyDown(KeyCode.P))
        {
            GameFlow.Instance.CallPauseMenu();
        }

        Vector3 currentPosition = transform.position;
        Vector3 actualMovement = currentPosition - lastPreviousPosition;
        if (actualMovement.magnitude > 0.01f)
        {
            lastMovementDirection = actualMovement.normalized;

        }

        if (inZone && Input.GetKeyDown(KeyCode.E))
        {
            GameFlow.Instance.LoadLevel();
        }

        lastPreviousPosition = currentPosition;
    }

    public void UpdateHintText(string hint)
    {
        hintText.text = hint;
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
        float mouseX = Input.GetAxis("Mouse X") * mouseScale * Time.unscaledDeltaTime;
        float mouseY = -Input.GetAxis("Mouse Y") * rotationSpeedY * mouseScale * Time.unscaledDeltaTime;
        //Debug.Log($"{nameof(mouseX)} {mouseX}; {nameof(mouseY)} {mouseY};");
        cameraHolder.localEulerAngles += new Vector3(0, mouseX, 0) * rotationSpeedY;

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


    public void Climb()
    {
        rb.velocity = Vector3.zero;

        Vector2 input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        bool inputHeld = input != Vector2.zero;

        Vector3 inputDir = (headCamera.forward * input.y + headCamera.right * input.x).normalized;
        float maxDistance = inputHeld ? handMaxDistance + handDistanceOffset : handMaxDistance;

        Vector3 toHands = hands.transform.position - transform.position;


        if (inputHeld)
        {

            Vector3 delta = transform.position + inputDir * climbingSpeed * Time.deltaTime; ;
            Vector3 offsetFromHand = delta - hands.transform.position;

            if (offsetFromHand.magnitude > maxDistance)
            {
                Vector3 tangentMove = Vector3.ProjectOnPlane(inputDir * climbingSpeed * Time.deltaTime, toHands.normalized);
                Vector3 spherePosition = transform.position + tangentMove;

                Vector3 correctedPosition = hands.transform.position + (spherePosition - hands.transform.position).normalized * maxDistance;
                rb.MovePosition(correctedPosition);
            }
            else
            {
                rb.MovePosition(delta);
            }

            //If player doesn't move add small movement along the sphere
        }
        else
        {
            float overshoot = toHands.magnitude - maxDistance;
            if (overshoot > 0)
            {
                Vector3 correction = toHands.normalized * Mathf.Min(overshoot, climbingSpeed * Time.deltaTime * 1.5f);
                rb.MovePosition(transform.position + correction);
            }
        }
    }


    public RaycastHit HandsRay()
    {
        Vector3 origin = headCamera.position + headCamera.forward * 0.5f;
        Vector3 direction = headCamera.forward;

        const float rayDistance = 2f;

        Physics.Raycast(origin, direction, out RaycastHit hit, rayDistance, climbCollisions);
        return hit;
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

    public IEnumerator MoveHandsToCliff(Vector3 targetPoint)
    {
        rb.velocity = Vector3.zero;
        //Quaternion previousRotation = transform.rotation;
        while (true)
        {
            Vector3 direction = (targetPoint - hands.transform.position).normalized;
            float distanceToTarget = Vector3.Distance(hands.transform.position, targetPoint);
            float step = magnetSpeed * Time.deltaTime;

            isCliffCoroutineRunning = true;
            if (distanceToTarget <= 0.2f)
                break;

            if (Physics.Raycast(hands.transform.position, direction, step + 0.1f, Physics.AllLayers))
                break;

            hands.transform.position += direction * Mathf.Min(step, distanceToTarget);
            // transform.rotation = Quaternion.Slerp(previousRotation, targetRotation, Time.deltaTime * 5f);
            // previousRotation = transform.rotation;
            yield return null;
        }
        isCliffCoroutineRunning = false;
        //transform.rotation = targetRotation;
    }

}