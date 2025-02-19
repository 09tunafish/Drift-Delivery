using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public GameObject boxPrefab;
    public Transform[] deliveryPoints;

    private GameObject currentBox;
    private GameObject currentDeliveryPoint;
    private int currentDeliveryIndex = 0;
    public ArrowIndicator arrowIndicator;

    private void Start()
    {
        SpawnBoxAndDeliveryPoint();
    }

    public void SpawnBoxAndDeliveryPoint()
    {
        Vector2 boxPosition = new Vector2(Random.Range(-348f, 371f), Random.Range(-163f, 244f));
        currentBox = Instantiate(boxPrefab, boxPosition, Quaternion.identity);

        currentDeliveryPoint = Instantiate(deliveryPoints[currentDeliveryIndex].gameObject, deliveryPoints[currentDeliveryIndex].position, Quaternion.identity);

        currentDeliveryIndex = (currentDeliveryIndex + 1) % deliveryPoints.Length;
        arrowIndicator.SetTarget(currentBox.transform);
    }

    [System.Obsolete]
    public void BoxDelivered()
    {
        FindObjectOfType<ScoreManager>().AddScore(1);  

        Destroy(currentBox);
        Destroy(currentDeliveryPoint);

        arrowIndicator.SetTarget(null);

        SpawnBoxAndDeliveryPoint();
    }

    public void BoxPickedUp()
    {
        arrowIndicator.SetTarget(currentDeliveryPoint.transform);
        Debug.Log("PickedUp");
    }
}
