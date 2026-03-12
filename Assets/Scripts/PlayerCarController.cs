using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class PlayerCarController : MonoBehaviour
{
    [Header("Movimiento lateral (giroscopio)")]
    [SerializeField] private float laneWidth = 4f;
    [SerializeField] private float maxHorizontalOffset = 5f;
    [SerializeField] private float lateralSpeed = 6f;
    [SerializeField] private float tiltSensitivity = 2.2f;

    [Header("Velocidad avance")]
    [SerializeField] private float normalSpeed = 16f;
    [SerializeField] private float turboSpeed = 26f;

    [Header("Choque")]
    [SerializeField] private float spinDuration = 0.8f;
    [SerializeField] private AnimationCurve spinCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    private Rigidbody rb;
    private bool gyroEnabled;
    private Quaternion baseRotation;
    private bool recoveringFromCrash;

    public float CurrentForwardSpeed { get; private set; }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        EnableGyroscope();
    }

    private void Update()
    {
        if (GameManager.Instance == null || GameManager.Instance.IsGameOver || recoveringFromCrash)
        {
            CurrentForwardSpeed = 0f;
            return;
        }

        bool turbo = Input.touchCount > 0 && Input.GetTouch(0).phase != TouchPhase.Ended;
        GameManager.Instance.SetTurbo(turbo);
        CurrentForwardSpeed = turbo ? turboSpeed : normalSpeed;

        float targetX = GetHorizontalTarget();
        Vector3 targetPosition = transform.position;
        targetPosition.x = Mathf.Clamp(targetX, -maxHorizontalOffset, maxHorizontalOffset);

        Vector3 moved = Vector3.MoveTowards(transform.position, targetPosition, lateralSpeed * Time.deltaTime);
        transform.position = moved;
    }

    private void FixedUpdate()
    {
        if (GameManager.Instance == null || GameManager.Instance.IsGameOver || recoveringFromCrash)
        {
            return;
        }

        rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, CurrentForwardSpeed);
    }

    public void Crash()
    {
        if (!gameObject.activeInHierarchy || recoveringFromCrash || GameManager.Instance.IsGameOver)
        {
            return;
        }

        StartCoroutine(CrashRoutine());
    }

    private IEnumerator CrashRoutine()
    {
        recoveringFromCrash = true;
        float elapsed = 0f;
        Quaternion startRotation = transform.rotation;
        Quaternion endRotation = startRotation * Quaternion.Euler(0f, 360f, 0f);

        while (elapsed < spinDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / spinDuration);
            transform.rotation = Quaternion.Slerp(startRotation, endRotation, spinCurve.Evaluate(t));
            rb.velocity = Vector3.zero;
            yield return null;
        }

        transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f);
        recoveringFromCrash = false;
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

    private float GetHorizontalTarget()
    {
        if (gyroEnabled)
        {
            Quaternion delta = Quaternion.Inverse(baseRotation) * Input.gyro.attitude;
            float tilt = -delta.eulerAngles.z;
            if (tilt > 180f) tilt -= 360f;
            return (tilt / 30f) * laneWidth * tiltSensitivity;
        }

        float simulatedTilt = Input.GetAxis("Horizontal");
        return simulatedTilt * laneWidth;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle") || other.CompareTag("Traffic"))
        {
            Crash();
        }
    }
}
