using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopDownCarController : MonoBehaviour
{
    [Header("Car Settings")]
    public float driftFactor = 0.95f;
    public float accelerationFactor = 30.0f;
    public float turnFactor = 3.5f;
    public float maxSpeed = 20;
    private bool hasBox = false;
    private SpawnManager spawnManager;

    float accelerationInput = 0;
    float steeringInput = 0;
    float rotationAngle = 0;
    float velocityVsUp = 0;

    Rigidbody2D carRigidbody2D;

    void Awake()
    {
        carRigidbody2D = GetComponent<Rigidbody2D>();
    }

    [System.Obsolete]
    private void Start()
    {
        spawnManager = FindObjectOfType<SpawnManager>();
    }

    [System.Obsolete]
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Box") && !hasBox)
        {
            hasBox = true;
            Destroy(other.gameObject); 
            Debug.Log("Box picked up!");
            spawnManager.BoxPickedUp();
        }

        if (other.CompareTag("DeliveryPoint") && hasBox)
        {
            hasBox = false;
            Debug.Log("Box delivered!");
            spawnManager.BoxDelivered();
        }
    }

    void FixedUpdate()
    {
        ApplyEngineForce();

        KillOrthogonalVelocity();

        ApplySteering();
    }

    void ApplyEngineForce()
    {
        velocityVsUp = Vector2.Dot(transform.right, carRigidbody2D.linearVelocity); 

        if (velocityVsUp > maxSpeed && accelerationInput < 0)
            return;

        if (velocityVsUp < -maxSpeed * 0.5f && accelerationInput < 0)
            return;

        if (carRigidbody2D.linearVelocity.sqrMagnitude > maxSpeed * maxSpeed && accelerationInput > 0)
            return;

        if (accelerationInput == 0)
            carRigidbody2D.linearDamping = Mathf.Lerp(carRigidbody2D.linearDamping, 3.0f, Time.fixedDeltaTime * 3);
        else carRigidbody2D.linearDamping = 0;
        
        Vector2 engineForceVector = transform.right * accelerationInput * accelerationFactor;

        carRigidbody2D.AddForce(engineForceVector, ForceMode2D.Force);
    }

    void ApplySteering()
    {
        float minTurnSpeed = (carRigidbody2D.linearVelocity.magnitude / 8);
        minTurnSpeed = Mathf.Clamp01(minTurnSpeed);
        
        rotationAngle -= steeringInput * turnFactor * minTurnSpeed;

        carRigidbody2D.MoveRotation(rotationAngle - 90f);
    }

    void KillOrthogonalVelocity()
    {
        Vector2 forwardVelocity = transform.right * Vector2.Dot(carRigidbody2D.linearVelocity, transform.right);
        Vector2 rightVelocity = transform.up * Vector2.Dot(carRigidbody2D.linearVelocity, transform.up);

        carRigidbody2D.linearVelocity = forwardVelocity + rightVelocity * driftFactor;
    }

    float GetLateralVelocity()
    {
        return Vector2.Dot(transform.up, carRigidbody2D.linearVelocity);
    }

    public bool IsTireScreeching(out float lateralVelocity, out bool isBraking)
    {
        lateralVelocity = GetLateralVelocity();
        isBraking = false;

        if (accelerationInput < 0 && velocityVsUp > 0)
        {
            isBraking = true;
            return true;
        }

        if (accelerationInput > 0 && velocityVsUp == 0)
        return true;

        if (accelerationInput > 0 && velocityVsUp < 0)
        return true;

        if (Mathf.Abs(GetLateralVelocity()) >4.0f)
        return true;

        return false;
    }

    public void SetInputVector(Vector2 inputVector)
    {
        steeringInput = inputVector.x;
        accelerationInput = inputVector.y;
    }

    public float GetVelocityMagnitude()
    {
        return carRigidbody2D.linearVelocity.magnitude;
    }
}