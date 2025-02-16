using System.Xml;
using System.Collections.Generic;
using UnityEngine;

public class OSMImporter : MonoBehaviour
{
    public TextAsset osmFile;
    public float scale = 0.001f;  // Adjust scale value for better visibility
    public Material roadMaterial;

    private Dictionary<string, Vector2> nodePositions = new Dictionary<string, Vector2>();

    void Start()
    {
        if (osmFile != null)
        {
            LoadOSMData(osmFile.text);
        }
        else
        {
            Debug.LogError("No OSM file assigned.");
        }
    }

    void LoadOSMData(string xmlData)
    {
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(xmlData);

        CacheNodes(xmlDoc);

        XmlNodeList wayNodes = xmlDoc.GetElementsByTagName("way");
        foreach (XmlNode way in wayNodes)
        {
            bool isRoad = false;
            List<Vector2> points = new List<Vector2>();

            foreach (XmlNode tag in way.ChildNodes)
            {
                if (tag.Name == "tag" && tag.Attributes["k"].Value == "highway")
                {
                    isRoad = true;
                    Debug.Log("Road found with id: " + way.Attributes["id"].Value);
                    break;
                }
            }

            if (!isRoad) continue;

            foreach (XmlNode nd in way.ChildNodes)
            {
                if (nd.Name == "nd")
                {
                    string refId = nd.Attributes["ref"].Value;
                    if (nodePositions.ContainsKey(refId))
                    {
                        points.Add(nodePositions[refId]);
                    }
                }
            }

            if (points.Count >= 2)
            {
                CreateRoadMesh(points);
            }
            else
            {
                Debug.LogWarning("Way with ID " + way.Attributes["id"].Value + " has less than 2 points.");
            }
        }
    }

    void CacheNodes(XmlDocument xmlDoc)
    {
        XmlNodeList nodeList = xmlDoc.GetElementsByTagName("node");
        foreach (XmlNode node in nodeList)
        {
            string id = node.Attributes["id"].Value;
            float lat = float.Parse(node.Attributes["lat"].Value);
            float lon = float.Parse(node.Attributes["lon"].Value);
            Vector2 pos = new Vector2(lon * scale, lat * scale);
            nodePositions[id] = pos;

            // Debugging node positions
            Debug.Log("Node position: " + pos);
        }
    }

    void CreateRoadMesh(List<Vector2> points)
{
    // Find the bounds of the road
    Vector2 min = points[0];
    Vector2 max = points[0];

    foreach (var point in points)
    {
        if (point.x < min.x) min.x = point.x;
        if (point.x > max.x) max.x = point.x;
        if (point.y < min.y) min.y = point.y;
        if (point.y > max.y) max.y = point.y;
    }

    // Calculate the road width and height
    float roadWidth = max.x - min.x;
    float roadHeight = max.y - min.y;

    // Choose a scale factor based on the road's bounds
    float scaleFactor = Mathf.Max(roadWidth, roadHeight) * 0.1f; // Adjust 0.1f as needed

    // Create the road GameObject
    GameObject road = new GameObject("RoadMesh");
    road.transform.position = new Vector3(-min.x * scaleFactor, -min.y * scaleFactor, 0);  // Offset to center around (0,0,0)

    MeshFilter meshFilter = road.AddComponent<MeshFilter>();
    MeshRenderer meshRenderer = road.AddComponent<MeshRenderer>();
    meshRenderer.material = roadMaterial != null ? roadMaterial : new Material(Shader.Find("Standard"));

    Mesh mesh = new Mesh();
    Vector3[] vertices = new Vector3[points.Count * 2];
    int[] triangles = new int[(points.Count - 1) * 6];

    for (int i = 0; i < points.Count; i++)
    {
        Vector2 point = points[i];
        Vector2 normal = (i < points.Count - 1) ? Vector2.Perpendicular(points[i + 1] - point).normalized : Vector2.Perpendicular(point - points[i - 1]).normalized;

        // Scale the points and adjust Z=0 for 2D
        vertices[i * 2] = new Vector3((point.x - min.x) * scaleFactor + normal.x * 0.1f, (point.y - min.y) * scaleFactor + normal.y * 0.1f, 0);
        vertices[i * 2 + 1] = new Vector3((point.x - min.x) * scaleFactor - normal.x * 0.1f, (point.y - min.y) * scaleFactor - normal.y * 0.1f, 0);
    }

    for (int i = 0; i < points.Count - 1; i++)
    {
        int idx = i * 6;
        int v = i * 2;

        triangles[idx] = v;
        triangles[idx + 1] = v + 2;
        triangles[idx + 2] = v + 1;
        triangles[idx + 3] = v + 1;
        triangles[idx + 4] = v + 2;
        triangles[idx + 5] = v + 3;
    }

    mesh.vertices = vertices;
    mesh.triangles = triangles;
    mesh.RecalculateNormals();
    mesh.RecalculateBounds();
    meshFilter.mesh = mesh;

    Debug.Log("Road mesh generated with " + vertices.Length + " vertices.");
}


    // Debugging with Gizmos to show node positions in the editor
    void OnDrawGizmos()
    {
        if (nodePositions.Count > 0)
        {
            Gizmos.color = Color.red;
            foreach (var node in nodePositions.Values)
            {
                Gizmos.DrawSphere(new Vector3(node.x, node.y, 0), 0.1f);
            }
        }
    }
}
