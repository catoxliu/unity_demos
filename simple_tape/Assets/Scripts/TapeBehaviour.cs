using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TapeBehaviour : MonoBehaviour
{

    public float mfTapeIncrement = 0.003f;

    private Mesh mTapeMesh;
    private float mfTapeWidth = 0.01f;
    private float mfDefaultTapeHeight = 0.1f;
    private float mfTapeRotateSpeed = -1;
    private GameObject mTape;

    // Use this for initialization
    void Start()
    {
        mTape = transform.GetChild(0).gameObject;
        mTapeMesh = new Mesh();
        mTapeMesh.vertices = new Vector3[] { new Vector3(0, 0, 0), new Vector3(-mfTapeWidth, 0, 0), new Vector3(0, mfDefaultTapeHeight, 0), new Vector3(-mfTapeWidth, mfDefaultTapeHeight, 0), new Vector3(-mfTapeWidth, 0, 0) };
        mTapeMesh.uv = new Vector2[] { new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 1), new Vector2(1, 0), new Vector2(0, 0) };
        mTapeMesh.triangles = new int[] { 0, 1, 2, 1, 3, 2, 0, 4, 1 };
        MeshFilter mf = mTape.AddComponent<MeshFilter>();
        MeshRenderer mr = mTape.AddComponent<MeshRenderer>();
        mf.sharedMesh = mTapeMesh;

        Material defaultTapeMat = Resources.Load("Materials/DefaultTapeMaterial") as Material;
        mr.material = defaultTapeMat;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            Vector3[] vers = mTapeMesh.vertices;
            vers[2].y = vers[2].y + mfTapeIncrement;
            vers[3].y = vers[3].y + mfTapeIncrement;
            mTapeMesh.vertices = vers;
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            StartCoroutine(TapeWrap());
        }
    }

    IEnumerator TapeWrap()
    {
        var pg = GameObject.FindObjectOfType<PolygonGenerator>();
        var startPoint = Vector2.zero;
        var tapeVertices = mTapeMesh.vertices;
        var tapeUVs = mTapeMesh.uv;
        var tapeTriangles = mTapeMesh.triangles;

        var convexHull = GetConvexHull(pg.mPolygonVertices);
        Debug.Log(convexHull.Count);

        for (int i = 0; i <= pg.mPolygonVertices.Length; i++)
        {
            var vertice = i == pg.mPolygonVertices.Length ? Vector2.zero : pg.mPolygonVertices[i];

            var tapeLength = (tapeVertices[2] - tapeVertices[0]).magnitude;

            if (!convexHull.Contains(vertice))
            {
                Debug.Log("Concave point!");
                var idx = i;
                List<Vector2> concavePoints = new List<Vector2>();
                concavePoints.Add(vertice);
                while (idx < pg.mPolygonVertices.Length)
                {
                    idx++;
                    vertice = idx == pg.mPolygonVertices.Length ? Vector2.zero : pg.mPolygonVertices[idx];
                    if (convexHull.Contains(vertice))
                        break;
                    concavePoints.Add(vertice);
                }
                i = idx;

                var convexEdge = vertice - startPoint;
                var convexEdgeLength = convexEdge.magnitude;

                concavePoints.Sort((Vector2 a, Vector2 b) =>
                    {
                        var c = Vector2.Distance(a, startPoint) * Mathf.Sin(
                            Vector2.Angle(convexEdge, a - startPoint) * Mathf.Deg2Rad)
                        - Vector2.Distance(b, startPoint) * Mathf.Sin(
                            Vector2.Angle(convexEdge, b - startPoint) * Mathf.Deg2Rad);
                        return c < 0 ? -Mathf.CeilToInt(Mathf.Abs(c)) : Mathf.CeilToInt(c);
                    });

                idx = 0;
                if (tapeLength <= convexEdgeLength)
                {

                }
            }


            var edge = vertice - startPoint;
            var length = edge.magnitude;

            bool bBreak = tapeLength <= length;
            var wrapVertice = Vector3.MoveTowards(tapeVertices[0], tapeVertices[2], bBreak ? tapeLength : length);

            if (wrapVertice.x == vertice.x && wrapVertice.y == vertice.y)
            {
                //break tape mesh
                BreakMesh(wrapVertice, ref tapeVertices);
            }
            else
            {
                //rotate the tape
                var angle = Vector3.SignedAngle(wrapVertice - tapeVertices[0], edge, Vector3.back);
                Debug.Log(angle);
                var pivot = tapeVertices[0];
                while (angle > 1)
                {
                    tapeVertices[1] = RotatePointAroundPivod(tapeVertices[1], pivot, mfTapeRotateSpeed);
                    tapeVertices[2] = RotatePointAroundPivod(tapeVertices[2], pivot, mfTapeRotateSpeed);
                    tapeVertices[3] = RotatePointAroundPivod(tapeVertices[3], pivot, mfTapeRotateSpeed);
                    mTapeMesh.vertices = tapeVertices;
                    angle--;
                    yield return null;
                }
                tapeVertices[1] = RotatePointAroundPivod(tapeVertices[1], pivot, -angle);
                tapeVertices[2] = RotatePointAroundPivod(tapeVertices[2], pivot, -angle);
                tapeVertices[3] = RotatePointAroundPivod(tapeVertices[3], pivot, -angle);
                mTapeMesh.vertices = tapeVertices;
                yield return null;
                //break tape mesh
                if (bBreak) yield break;
                BreakMesh(vertice, ref tapeVertices);
            }

            startPoint = vertice;
        }
    }

    void BreakMesh(Vector3 wrapVertice, ref Vector3[] tapeVertices)
    {
        var wrapMoveDir = wrapVertice - tapeVertices[0];

        GameObject stackTape = Instantiate(mTape, transform);
        Mesh stackTapeMesh = stackTape.GetComponent<MeshFilter>().mesh;
        var stackMeshVers = stackTapeMesh.vertices;
        stackMeshVers[2] = wrapVertice;
        stackMeshVers[3] = tapeVertices[1] + wrapMoveDir;
        stackTapeMesh.vertices = stackMeshVers;

        tapeVertices[0] = wrapVertice;
        tapeVertices[1] = tapeVertices[1] + wrapMoveDir;
        tapeVertices[4] = tapeVertices[1];

        mTapeMesh.vertices = tapeVertices;
    }

    Vector3 RotatePointAroundPivod(Vector3 p, Vector3 pivot, float angle)
    {
        var dir = p - pivot;
        dir = Quaternion.Euler(0, 0, angle) * dir;
        p = dir + pivot;
        return p;
    }

    List<Vector2> GetConvexHull(Vector2[] vertices)
    {
        var points = new Vector2[vertices.Length];
        vertices.CopyTo(points, 0);
        System.Array.Sort(points, (Vector2 a, Vector2 b) =>
        {
            var c = Vector2.SignedAngle(Vector2.right, a) - Vector2.SignedAngle(Vector2.right, b);
            return c < 0 ? -(Mathf.CeilToInt(Mathf.Abs(c))) : Mathf.CeilToInt(c);
        }
            );
        List<Vector2> result = new List<Vector2>();
        result.Add(Vector2.zero);
        foreach (var p in points)
        {
            if (result.Count == 1)
            {
                result.Add(p);
                continue;
            }

            while (Vector2.SignedAngle(result[result.Count - 1] - result[result.Count - 2], p - result[result.Count - 2]) < 0)
            {
                result.RemoveAt(result.Count - 1);
                if (result.Count == 1)
                    break;
            }
            result.Add(p);
        }

        result.Reverse();
        return result;
    }
}
