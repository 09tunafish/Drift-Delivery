using UnityEngine;

public class TopDownCarController : MonoBehaviour
{
    [Header("Car Settings")]
    public float driftFactor = 0.95f;
    public float accelerationFactor = 30.0f;
    public float turnFactor = 3.5f;

    float accelerationInput = 0;
    float steeringInput = 0;
    float rotationAngle = 0;

    Rigidbody2D carRigidbody2D;

    void Awake()
    {
        carRigidbody2D = GetComponent<Rigidbody2D>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        ApplyEngineForce();

        KillOrthogonalVelocity();

        ApplySteering();
    }

    void ApplyEngineForce()
    {
        Vector2 engineForceVector = transform.right * accelerationInput * accelerationFactor;

        carRigidbody2D.AddForce(engineForceVector, ForceMode2D.Force);
    }

    void ApplySteering()
    {
        float minSpeedBeforeAllowingTurningFactor = (carRigidbody2D.linearVelocity.magnitude / 8);
        minSpeedBeforeAllowingTurningFactor = Mathf.Clamp01(minSpeedBeforeAllowingTurningFactor);
        
        rotationAngle -= steeringInput * turnFactor;

        carRigidbody2D.MoveRotation(rotationAngle - 90f);
    }

    void KillOrthogonalVelocity()
    {
        Vector2 forwardVelocity = transform.right * Vector2.Dot(carRigidbody2D.linearVelocity, transform.right);
        Vector2 rightVelocity = transform.up * Vector2.Dot(carRigidbody2D.linearVelocity, transform.up);

        carRigidbody2D.linearVelocity = forwardVelocity + rightVelocity * driftFactor;
    }

    public void SetInputVector(Vector2 inputVector)
    {
        steeringInput = inputVector.x;
        accelerationInput = inputVector.y;
    }
}