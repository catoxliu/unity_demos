using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PolygonGenerator : MonoBehaviour {

    public Vector2[] mPolygonVertices = new Vector2[] { };

    private float mEdgeWidth = 0.01f;

	// Use this for initialization
	void Start () {
        Material defaultEdgeMat = Resources.Load("Materials/DefaultEdgeMaterial") as Material;

        var startPoint = Vector2.zero;
        var lastPoint = mPolygonVertices[mPolygonVertices.Length - 1];
        int hollowVerticesCount = mPolygonVertices.Length * 2 + 2;
        Vector3[] hollowPolygonVertices = new Vector3[hollowVerticesCount];
        Vector2[] hollowPolygonUVs = new Vector2[hollowVerticesCount];
        int[] hollowTriangles = new int[hollowVerticesCount * 3];
        for (int i = 0; i <= mPolygonVertices.Length; i++)
        {
            var vertice = i == mPolygonVertices.Length ? Vector2.zero : mPolygonVertices[i];

            var leftEdge = vertice - startPoint;
            var rightEdge = lastPoint - startPoint;
            var angle = Vector2.Angle(leftEdge, rightEdge);
            //Debug.Log(angle);
            var ratio = mEdgeWidth / Mathf.Sin(angle / 2.0f * Mathf.Deg2Rad);
            //Debug.Log(ratio);
            var hollowFactor = leftEdge.normalized + rightEdge.normalized;
            hollowFactor = (ratio / hollowFactor.magnitude) * hollowFactor;
            //handle Concave situations
            var cross = Vector3.Cross(-rightEdge, leftEdge);
            if (cross.z > 0)
                hollowFactor = -hollowFactor;
            //Debug.Log(hollowFactor.x + " | " + hollowFactor.y);

            hollowPolygonVertices[i * 2] = startPoint;
            hollowPolygonVertices[i*2+1] = (startPoint + hollowFactor);
            hollowPolygonUVs[i * 2] = new Vector2(i % 2, i % 2);
            hollowPolygonUVs[i * 2 + 1] = new Vector2(i%2, (i+1)%2);
            hollowTriangles[i*6] = i*2;
            hollowTriangles[i * 6 + 1] = i * 2 + 2 >= hollowVerticesCount ? 0 : i * 2 + 2;
            hollowTriangles[i * 6 + 2] = i * 2 + 1;
            
            hollowTriangles[i * 6 + 3] = i * 2 + 1;
            hollowTriangles[i * 6 + 4] = i * 2 + 2 >= hollowVerticesCount ? 0 : i * 2 + 2;
            hollowTriangles[i * 6 + 5] = i * 2 + 3 >= hollowVerticesCount ? 1 : i * 2 + 3;
            
            lastPoint = startPoint;
            startPoint = vertice;
        }
        Mesh m = new Mesh();
        m.vertices = hollowPolygonVertices;
        m.uv = hollowPolygonUVs;
        m.triangles = hollowTriangles;

        MeshFilter mf = gameObject.AddComponent<MeshFilter>();
        MeshRenderer mr = gameObject.AddComponent<MeshRenderer>();
        mf.sharedMesh = m;

        mr.material = defaultEdgeMat;


    }
	
}
