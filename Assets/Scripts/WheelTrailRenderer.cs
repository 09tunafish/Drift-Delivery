using UnityEngine;

public class WheelTrailRenderer : MonoBehaviour
{

    TopDownCarController topDownCarController;
    TrailRenderer trailRenderer;

    void Awake()
    {
        topDownCarController = GetComponentInParent<TopDownCarController>();

        trailRenderer = GetComponent<TrailRenderer>();

        trailRenderer.emitting = false;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (topDownCarController.IsTireScreeching(out float lateralVelocity, out bool isBraking))
            trailRenderer.emitting = true;
        else trailRenderer.emitting = false;
    }
}
