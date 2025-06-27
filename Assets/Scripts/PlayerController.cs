using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float playerCameraFOV = 60;
    [SerializeField] private float baseSpeed;
    [SerializeField] private float rotationSpeedY, rotationSpeedX;
    [SerializeField] private float jumpHeight;
    [SerializeField] private float grabDistance;

    [SerializeField] private Transform headCamera;
    [SerializeField] private Transform cameraHolder;
    [SerializeField] private Rigidbody rb;
 //   [SerializeField] private SimpleGroundChecker groundChecker;
    [SerializeField] private Collider mainCollider;

    [SerializeField] private float normalFootDelay, boostFootDelay;


    private Camera viewCamera;
    private float speed;
    public float mouseScale = 1.0f;
    private float doubleJumpTimer;
    public float BaseSpeed => baseSpeed;
    public float CurrentSpeed => speed;

    private Vector3 cashedMousePosition;


    private void Start()
    {

        speed = baseSpeed;
        viewCamera = headCamera.GetComponent<Camera>();

        _currentFootTimer = normalFootDelay;
        _currentDelay = _currentFootTimer;


    }

    private float _currentFootTimer;
    private float _currentDelay;
    private void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
        {
            Move();

            if (IsGrounded())
            {
                _currentDelay -= UnityEngine.Time.fixedDeltaTime;
            }

            if (_currentDelay <= 0)
            {
                _currentDelay = _currentFootTimer;
            }
        }
    }

    bool IsGrounded()
    {

        return Physics.Raycast(transform.position, -Vector3.up, mainCollider.bounds.size.y / 2);
    }

    bool CouldGrabSurface()
    {
        return Physics.Raycast(headCamera.position, headCamera.forward, grabDistance);
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

private void Move()
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
        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        rb.AddForce(Vector3.up * height, ForceMode.Impulse);
    }

    private void OnGUI()
    {

        //GUI.Label(new Rect(10, 10, 120, 60), $"Remaining time: {timer.CurrentLifeTime}");
    }
}