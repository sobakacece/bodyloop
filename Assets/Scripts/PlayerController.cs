using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float rotationSpeedY;
    [SerializeField] public float jumpHeight;
    [SerializeField] public float grabDistance = 2.0f;

    [SerializeField] public Transform headCamera;
    [SerializeField] public Transform cameraHolder;
    [SerializeField] public Rigidbody rb;
    //   [SerializeField] private SimpleGroundChecker groundChecker;
    [SerializeField] private Collider mainCollider;

    public StateMachine stateMachine;

    [SerializeField] public LayerMask climbCollisions;
    [SerializeField] public ClimbingHand[] hands;


    public float mouseScale = 1.0f;

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
            playerHUD.ImageProgress = currentStamina / maxStamina;
        }
    }
    private float currentStamina;
    public bool staminaDepleted = false;
    public bool isCliffCoroutineRunning = false;
    [SerializeField] public float staminaReduceSpeed = 10.0f;
    [SerializeField] public float staminaRecoverySpeed = 20.0f;

    [SerializeField] public float handMaxDistance = 0.7f;
    [SerializeField] public float handDistanceOffset = 0.3f;
    [SerializeField] public float lerpCorrectionSpeed = 8.0f;


    public Vector3 spawnPoint;
    public Quaternion spawnRotation;

    public Vector3 handsLocalSpawnPosition;
    public Quaternion handsLocalSpawnRotation;

    [SerializeField] private GameObject hudPrefab;
    [SerializeField] private PlayerHUD playerHUD;


    public Vector3 lastMovementDirection;
    private Vector3 lastPreviousPosition;

    public Vector3 lastMagnetPosition = Vector3.zero;
    public bool inZone;
    [SerializeField]

    void Awake()
    {
        spawnPoint = transform.position;
        spawnRotation = transform.rotation;

    }
    private void Start()
    {
        GameObject hudInstance = Instantiate(hudPrefab);
        //.transform.SetParent(hands.transform);
        playerHUD = hudInstance.GetComponent<PlayerHUD>();


        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        MyCurrentStamina = maxStamina;
        stateMachine = GetComponent<StateMachine>();

        GameFlow.Instance.sensetivityChanged += (float value) =>
        {
            mouseScale = value;
        };
        mouseScale = GameFlow.Instance.baseSens;

    }


    public bool IsGrounded()
    {
        float radius = mainCollider.bounds.extents.x - 0.2f; // approximate cylinder radius
        float checkDistance = 0.1f; // small extra distance to check just below the collider
        Vector3 origin = transform.position + Vector3.up * 0.1f; // lift origin slightly to avoid clipping

        return Physics.SphereCast(origin, radius, Vector3.down, out _,
            mainCollider.bounds.extents.y + checkDistance);
    }


    public bool CouldStartClimb()
    {
        return ForwardRay().collider != null;
    }


    private void Update()
    {
        //        Debug.Log(hands.transform.forward);
        //  PreventCornerSnag();

        if (Input.GetKeyDown(KeyCode.R))
            Restart();

        // Debug.Log(IsGrounded());
        if (Application.isFocused)
            Look();

        if (Input.GetKeyDown(KeyCode.P))
        {
            GameFlow.Instance.CallPauseMenu();
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            stateMachine.ChangeState(StateMachine.StateEnum.Death);
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
        playerHUD.UpdateCrossHair(CouldStartClimb());
    }

    public void UpdateHintText(string hint)
    {
        playerHUD.hintText.text = hint;
    }

    private void Restart()
    {
        GameFlow.Instance.GameRestart();
    }

    public void Move(float speed = 1)
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

        if (Mathf.Abs(q.x) <= 0.7)
            headCamera.localRotation = q;
    }

    public void Jump(float height)
    {
        //stateMachine.ChangeState(StateMachine.StateEnum.Fall);
        //rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        rb.AddForce(Vector3.up * height, ForceMode.Impulse);
    }


    public void Climb()
    {
        //    rb.velocity = Vector3.zero;

        Vector2 input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        bool inputHeld = input != Vector2.zero;
        Vector3 inputDir = (headCamera.forward * input.y + headCamera.right * input.x).normalized;

        float maxDistance = handMaxDistance + (inputHeld ? handDistanceOffset : 0f);

        Vector3 magnetPoint = FindMagnetPoint();
        if (magnetPoint == Vector3.zero)
            return;
        // prevent to snap to the 0 coordinate
        // climb state resets lastmagnetposition to Vector3.zero, so it updated on the first frame of climbing instead of lerping from zero
        if (lastMagnetPosition == Vector3.zero)
            lastMagnetPosition = magnetPoint;
        else
        {
            float handJump = Vector3.Distance(lastMagnetPosition, magnetPoint);

            // If hands change position drastically, smooth more aggressively
            if (handJump > maxDistance * 0.5f)
                lastMagnetPosition = Vector3.Lerp(lastMagnetPosition, magnetPoint, Time.deltaTime * (1.0f * 0.5f));
            else
                lastMagnetPosition = Vector3.Lerp(lastMagnetPosition, magnetPoint, 1.0f * Time.deltaTime);
        }

        Vector3 toHands = magnetPoint - transform.position;

        if (inputHeld)
        {
            Vector3 delta = transform.position + inputDir * climbingSpeed * Time.deltaTime;
            Vector3 target = delta;

            // if you are in range of hand -> move as intended with camera turned on
            if ((delta - magnetPoint).magnitude > maxDistance)
            {
                Vector3 tangentMove = Vector3.ProjectOnPlane(inputDir * climbingSpeed * Time.deltaTime, toHands.normalized);
                Vector3 spherePosition = transform.position + tangentMove;

                target = magnetPoint + (spherePosition - magnetPoint).normalized * maxDistance;

                // basicaly if you move too far from the hand -> you begin to move along the invisible sphere around the magnet point
            }

            rb.MovePosition(Vector3.Lerp(transform.position, target, lerpCorrectionSpeed));

            // If player doesn't move add small movement along the sphere
        }
        else
        {
            float overshoot = toHands.magnitude - maxDistance;
            if (overshoot > 0)
            {
                Vector3 correction = toHands.normalized * Mathf.Min(overshoot, climbingSpeed * Time.deltaTime * 1.5f);
                Vector3 target = transform.position + correction;
                rb.MovePosition(Vector3.Lerp(transform.position, target, lerpCorrectionSpeed));
            }

            // if you don't hold input -> magnet to your hand
        }
    }


    private Vector3 FindMagnetPoint()
    {
        //looking for magnet point for climb function. Basically either of hands or middle point between them. Vector.zero is a null point. Feels dull, but anyway
        Vector3 target = Vector3.zero;

        if (hands[0].isActive && hands[1].isActive)
        {
            target = (hands[0].transform.position + hands[1].transform.position) / 2f;
        }
        else if (hands[0].isActive)
        {
            target = hands[0].transform.position;
        }
        else if (hands[1].isActive)
        {
            target = hands[1].transform.position;
        }
        return target;
    }


    public RaycastHit ForwardRay()
    {
        Vector3 origin = headCamera.position;

        Physics.Raycast(origin, headCamera.forward, out RaycastHit hit, grabDistance, climbCollisions);
        Debug.DrawRay(origin, headCamera.forward.normalized * grabDistance, Color.red);
        return hit;
    }

    public void Respawn()
    {
        rb.velocity = Vector3.zero;
        transform.position = spawnPoint;
        transform.rotation = spawnRotation;
        currentStamina = maxStamina;

    }

    private void OnGUI()
    {
        GUIStyle style = new GUIStyle();
        style.fontSize = 24;
        style.normal.textColor = Color.white;
        GUI.Label(new Rect(40, 120, 400, 80), "left hand activity: " + hands[0].isActive, style);
        GUI.Label(new Rect(40, 200, 400, 80), "right hand activity: " + hands[1].isActive, style);
    }



}