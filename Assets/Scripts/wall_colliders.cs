using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class wall_colliders : MonoBehaviour
{
    public GameObject road;
    public Mesh mesh;

    public float wallHeight = 1.0f;
    public float wallWidth = 0.1f;

    public GameObject wall;

    // Start is called before the first frame update
    void Start()
    {
        getMeshEdges();
    }

    class Triangle {
        public Vector3 v1;
        public Vector3 v2;
        public Vector3 v3;
        public Triangle(Vector3 v1, Vector3 v2, Vector3 v3)
        {
            this.v1 = v1;
            this.v2 = v2;
            this.v3 = v3;
        }
    }

    class Edge {
        public Vector3 v1;
        public Vector3 v2;
        public Edge(Vector3 v1, Vector3 v2)
        {
            this.v1 = v1;
            this.v2 = v2;
        }
    }

    void getMeshEdges()
    {
        mesh = road.GetComponent<MeshFilter>().mesh;

        int[] triangles = mesh.triangles;
        Vector3[] vertices = mesh.vertices;

        List<Vector3> VertexList = new List<Vector3>();

        foreach (Vector3 vertex in vertices)
        {
            VertexList.Add(vertex + road.transform.position);
        }

        HashSet<Edge> edgeSet = new HashSet<Edge>();
        HashSet<Triangle> triangleSet = new HashSet<Triangle>();

        for (int i = 0; i < triangles.Length; i += 3)
        {
            Vector3 v1 = VertexList[triangles[i]];
            Vector3 v2 = VertexList[triangles[i + 1]];
            Vector3 v3 = VertexList[triangles[i + 2]];

            edgeSet.Add(new Edge(v1, v2));
            edgeSet.Add(new Edge(v2, v3));
            edgeSet.Add(new Edge(v3, v1));

            triangleSet.Add(new Triangle(v1, v2, v3));
        }

        Dictionary<Edge, int> edgeTriangleCount = new Dictionary<Edge, int>();

        foreach (Edge edge in edgeSet)
        {
            edgeTriangleCount.Add(edge, 0);
        }

        foreach (Triangle triangle in triangleSet)
        {
            foreach (Edge edge in edgeSet)
            {
                if (edgeTriangleCount.ContainsKey(edge))
                {
                    if (edge.v1 == triangle.v1 || edge.v1 == triangle.v2 || edge.v1 == triangle.v3)
                    {
                        if (edge.v2 == triangle.v1 || edge.v2 == triangle.v2 || edge.v2 == triangle.v3)
                        {
                            edgeTriangleCount[edge] += 1;
                        }
                    }
                }
            }
        }

        foreach (KeyValuePair<Edge, int> entry in edgeTriangleCount)
        {
            if (entry.Value == 1)
            {
                Debug.DrawLine(entry.Key.v1, entry.Key.v2, Color.red, 1000);
                instanceWall(entry.Key);
            }
        }
    }

    void instanceWall(Edge edge)
    {
        Vector3 wallPosition = (edge.v1 + edge.v2) / 2;
        Vector3 wallScale = new Vector3(wallWidth, wallHeight, Vector3.Distance(edge.v1, edge.v2));

        Quaternion wallRotation = Quaternion.FromToRotation(Vector3.forward, edge.v2 - edge.v1);

        GameObject wallInstance = Instantiate(wall, wallPosition, wallRotation);

        wallInstance.transform.localScale = wallScale;
        //wallInstance.transform.localEulerAngles = new Vector3(0, 0, 0);
    }
}
