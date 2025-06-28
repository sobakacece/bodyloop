using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Unity.VisualScripting;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float playerCameraFOV = 60;
    [SerializeField] private float baseSpeed;
    [SerializeField] private float rotationSpeedY, rotationSpeedX;
    [SerializeField] private float jumpHeight;
    [SerializeField] private float grabDistance;

    [SerializeField] public Transform headCamera;
    [SerializeField] private Transform cameraHolder;
    [SerializeField] private Rigidbody rb;
    //   [SerializeField] private SimpleGroundChecker groundChecker;
    [SerializeField] private Collider mainCollider;

    private PlayerStateMachine stateMachine;

    [SerializeField] private LayerMask climbCollisions;
    [SerializeField] private GameObject hands;


    private Camera viewCamera;
    private float speed;
    public float mouseScale = 1.0f;
    private float doubleJumpTimer;
    public float BaseSpeed => baseSpeed;
    public float CurrentSpeed => speed;

    private Coroutine cliffCoroutine;

    [SerializeField] private float magnetSpeed = 5.0f;

    [SerializeField] private float climbingSpeed = 10.0f;
    [SerializeField] public float maxStamina = 100.0f;
    [SerializeField] public float currentStamina;
    [SerializeField] public float staminaReduceSpeed = 10.0f;
    [SerializeField] public float staminaRecoverySpeed = 20.0f;

    public Vector3 spawnPoint;
    public Quaternion spawnRotation;

    void Awake()
    {
        spawnPoint = transform.position;
        spawnRotation = transform.rotation;
        Debug.Log(spawnPoint);
    }
    private void Start()
    {
        currentStamina = maxStamina;
        speed = baseSpeed;
        viewCamera = headCamera.GetComponent<Camera>();
        stateMachine = GetComponent<PlayerStateMachine>();

    }


    public bool IsGrounded()
    {
        return Physics.Raycast(transform.position, -Vector3.up, mainCollider.bounds.size.y);
    }

    public bool CouldGrabSurface()
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

        //Debug.Log(CouldGrabSurface());
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
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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
        //cameraAngle = Vector3.Angle(cameraHolder.forward, q.eulerAngles);

        if (Mathf.Abs(q.x) <= 0.7)
            headCamera.localRotation = q;
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

    public void TryGrabCliff()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, grabDistance, climbCollisions))
        {
            if (cliffCoroutine != null)
                StopCoroutine(cliffCoroutine);

            cliffCoroutine = StartCoroutine(MoveToCliff(hit.point));
        }
    }

    private IEnumerator MoveToCliff(Vector3 targetPoint)
    {
        while (true)
        {
            Vector3 direction = (targetPoint - transform.position).normalized;
            float distanceToTarget = Vector3.Distance(transform.position, targetPoint);
            float step = magnetSpeed * Time.deltaTime;

            if (distanceToTarget <= mainCollider.bounds.size.x / 2)
                break;

            if (Physics.Raycast(transform.position, direction, step + 0.1f, Physics.AllLayers))
                break;

            transform.position += direction * Mathf.Min(step, distanceToTarget);
            yield return null;
        }
    }

    public void StopCliffMove()
    {
        if (cliffCoroutine != null)
        {
            StopCoroutine(cliffCoroutine);
        }
    }

public void Climb()
{
        FixHandsRotation();

        Vector2 input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        Vector3 move = (hands.transform.right * input.x + hands.transform.up * input.y) * climbingSpeed;
        rb.velocity = Vector3.zero;
        rb.AddForce(move, ForceMode.VelocityChange);
}



    private void FixHandsRotation()
    {
        if (Physics.Raycast(hands.transform.position, hands.transform.forward, out RaycastHit hit, grabDistance, climbCollisions))
            hands.transform.rotation = Quaternion.LookRotation(-hit.normal, Vector3.up);
    }

}