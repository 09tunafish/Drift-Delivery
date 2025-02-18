using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public GameObject boxPrefab;
    public GameObject deliveryPointPrefab;
    public Vector2 spawnAreaMin;
    public Vector2 spawnAreaMax;

    private GameObject currentBox;
    private GameObject currentDeliveryPoint;

    private void Start()
    {
        SpawnBoxAndDeliveryPoint();
    }

    public void SpawnBoxAndDeliveryPoint()
    {
        Vector2 boxPosition = new Vector2(Random.Range(spawnAreaMin.x, spawnAreaMax.x), Random.Range(spawnAreaMin.y, spawnAreaMax.y));
        currentBox = Instantiate(boxPrefab, boxPosition, Quaternion.identity);

        Vector2 deliveryPosition;
        do
        {
            deliveryPosition = new Vector2(Random.Range(spawnAreaMin.x, spawnAreaMax.x), Random.Range(spawnAreaMin.y, spawnAreaMax.y));
        } while (Vector2.Distance(boxPosition, deliveryPosition) < 2.0f);

        currentDeliveryPoint = Instantiate(deliveryPointPrefab, deliveryPosition, Quaternion.identity);
    }

    public void BoxDelivered()
    {
        Destroy(currentBox);
        Destroy(currentDeliveryPoint);
        SpawnBoxAndDeliveryPoint();
    }
}
