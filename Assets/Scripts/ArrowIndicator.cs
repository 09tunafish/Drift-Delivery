using UnityEngine;

public class ArrowIndicator : MonoBehaviour
{
    public Transform target;  // Current target (box or delivery point)
    public float offsetDistance = 0f;

    private Transform player;

    private void Start()
    {
        player = transform.parent;
    }

    private void Update()
    {
        if (target != null)
        {
            // Point arrow towards target
            Vector2 direction = (Vector2)target.position - (Vector2)player.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
            transform.rotation = Quaternion.Euler(0, 0, angle);

            // Position arrow in front of the player
            transform.position = player.position + (Vector3)direction.normalized * offsetDistance;
        }
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        Debug.Log("Arrow target set to: " + (newTarget != null ? newTarget.name : "None"));
    }
}
