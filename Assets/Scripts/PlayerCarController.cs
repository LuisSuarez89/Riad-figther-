using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerCarController : MonoBehaviour
{
    [Header("Movimiento lateral (giroscopio)")]
    [SerializeField] private float laneWidth = 4f;
    [SerializeField] private float maxHorizontalOffset = 5f;
    [SerializeField] private float lateralSpeed = 6f;
    [SerializeField] private float tiltSensitivity = 2.2f;
    [SerializeField] private float steeringSmoothing = 12f;
    [SerializeField] private float visualTiltAngle = 12f;

    [Header("Velocidad avance")]
    [SerializeField] private float normalSpeed = 16f;
    [SerializeField] private float turboSpeed = 26f;
    [SerializeField] private float acceleration = 18f;

    [Header("Choque")]
    [SerializeField] private float spinDuration = 0.8f;
    [SerializeField] private float crashSlowdownMultiplier = 0.35f;
    [SerializeField] private float invulnerabilityAfterCrash = 1.1f;
    [SerializeField] private AnimationCurve spinCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    private Rigidbody rb;
    private bool gyroEnabled;
    private Quaternion baseRotation;
    private bool recoveringFromCrash;
    private float targetForwardSpeed;
    private float invulnerabilityTimer;
    private float currentXVelocity;

    public float CurrentForwardSpeed { get; private set; }

    private Vector3 startPosition;
    private Quaternion startRotation;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        EnableGyroscope();
        startPosition = transform.position;
        startRotation = transform.rotation;
    }

    private void Update()
    {
        if (GameManager.Instance == null || GameManager.Instance.IsGameOver)
        {
            CurrentForwardSpeed = 0f;
            return;
        }

        if (invulnerabilityTimer > 0f)
        {
            invulnerabilityTimer -= Time.deltaTime;
        }

        bool turbo = IsTurboPressed();
        GameManager.Instance.SetTurbo(turbo && !recoveringFromCrash);
        targetForwardSpeed = turbo ? turboSpeed : normalSpeed;

        if (recoveringFromCrash)
        {
            targetForwardSpeed *= crashSlowdownMultiplier;
        }

        CurrentForwardSpeed = Mathf.MoveTowards(CurrentForwardSpeed, targetForwardSpeed, acceleration * Time.deltaTime);

        if (!recoveringFromCrash && CurrentForwardSpeed > 0.1f)
        {
            float steerInput = GetSteeringInput();
            // Convertimos lateralSpeed en grados de giro por segundo (ej. lateralSpeed 6 = 60 grados)
            float turnDegreesPerSecond = lateralSpeed * 12f; 
            transform.Rotate(0f, steerInput * turnDegreesPerSecond * Time.deltaTime, 0f, Space.World);
        }

        GameManager.Instance.RegisterTravel(CurrentForwardSpeed);
    }

    private void FixedUpdate()
    {
        if (GameManager.Instance == null || GameManager.Instance.IsGameOver)
        {
            rb.velocity = Vector3.zero;
            return;
        }

        // Ahora el coche se mueve hacia donde esté mirando (su transform.forward local)
        Vector3 forwardVelocity = transform.forward * CurrentForwardSpeed;
        rb.velocity = new Vector3(forwardVelocity.x, rb.velocity.y, forwardVelocity.z);
    }

    public void Crash()
    {
        if (GameManager.Instance == null || !gameObject.activeInHierarchy || recoveringFromCrash || invulnerabilityTimer > 0f || GameManager.Instance.IsGameOver)
        {
            return;
        }

        GameManager.Instance.ApplyCrashPenalty();
        StartCoroutine(CrashRoutine());
    }

    private IEnumerator CrashRoutine()
    {
        recoveringFromCrash = true;
        invulnerabilityTimer = invulnerabilityAfterCrash;
        float elapsed = 0f;
        Quaternion startCrashRot = transform.rotation;
        Quaternion endCrashRot = startCrashRot * Quaternion.Euler(0f, 360f, 0f);

        while (elapsed < spinDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / spinDuration);
            transform.rotation = Quaternion.Slerp(startCrashRot, endCrashRot, spinCurve.Evaluate(t));
            yield return null;
        }

        recoveringFromCrash = false;
    }

    private bool IsTurboPressed()
    {
        if (Input.touchCount > 0)
        {
            return Input.GetTouch(0).phase != TouchPhase.Ended && Input.GetTouch(0).phase != TouchPhase.Canceled;
        }

        return Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
    }

    private void EnableGyroscope()
    {
        gyroEnabled = SystemInfo.supportsGyroscope;
        if (gyroEnabled)
        {
            Input.gyro.enabled = true;
            baseRotation = Input.gyro.attitude;
        }
    }

    private float GetSteeringInput()
    {
        if (gyroEnabled)
        {
            Quaternion delta = Quaternion.Inverse(baseRotation) * Input.gyro.attitude;
            float tilt = -delta.eulerAngles.z;
            if (tilt > 180f)
            {
                tilt -= 360f;
            }

            return Mathf.Clamp(tilt / 30f, -1f, 1f);
        }

        return Input.GetAxis("Horizontal");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle") || other.CompareTag("Traffic"))
        {
            Crash();
        }
    }
}
