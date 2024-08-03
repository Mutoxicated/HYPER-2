using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

class Vertex {
    public static Vertex NA = new Vertex(Vector3.zero, 0, 0, Color.black);

    public Vector3 pos;
    public int absoluteIndex;
    public int relativeIndex;

    public Color color;

    public void hide(Color color) {
        this.color.r += color.r;
        this.color.g += color.g;
        this.color.b += color.b;
    }

    public Vertex(Vector3 pos, int absIndex, int relIndex, Color color) {
        this.pos = pos;
        absoluteIndex = absIndex;
        relativeIndex = relIndex;
        this.color = color;
    }
}

class Triangle {
    public Vertex[] vertices = new Vertex[3];

    public Triangle(Vertex vert1, Vertex vert2, Vertex vert3) {
        vertices[0] = vert1;
        vertices[1] = vert2;
        vertices[2] = vert3;
    }

    public (Vector3, Vector3) GetEdgePositions(int nth) {
        if (nth == 2) {
            return (vertices[nth].pos, vertices[0].pos);
        }else {
            return (vertices[nth].pos, vertices[nth+1].pos);
        }
    }
}

public class AdvancedWire : MonoBehaviour
{
    [SerializeField] private MeshFilter meshFilter;
    [SerializeField, Range(0f,0.3f)] private float sensitivity = 0.025f;

    private Mesh mesh;
    private Color[] colors;

    private Dictionary<(Vector3, Vector3), List<Triangle>> triangleCommons = new Dictionary<(Vector3, Vector3), List<Triangle>>();

    private bool prebaked = false;

    [SerializeField] private bool onlyTriangles = false;
    [ContextMenu("Reset Values")]
    private void ResetValues() {
        triangleCommons.Clear();
        colors = new Color[0];
        if (mesh != null) {
            mesh.colors = colors;
        }
        prebaked = false;
    }

    [ContextMenu("Prebake")]
    private void Prebake() {
        triangleCommons.Clear();
        if (GetMesh(true) == -1) {
            Debug.LogWarning("Tried to prebake without a mesh renderer");
            return;
        }

        var vertices = mesh.vertices;
        colors = mesh.colors;

        if (colors.Length == 0) {
            colors = new Color[vertices.Length];
            Array.Fill(colors, Color.black);
        }

        var unparsedTriangles = mesh.triangles;
        // Debug.Log("-- CREATING TRIANGLES --");
        for (int i = 0; i < unparsedTriangles.Length; i += 3) {
            // Debug.Log("Tri "+(i+3)/3+":");
            // Debug.Log(unparsedTriangles[i]+", "+unparsedTriangles[i+1]+", "+unparsedTriangles[i+2]);
            Vertex vert1 = new Vertex(vertices[unparsedTriangles[i]], unparsedTriangles[i], 0, Color.red);
            Vertex vert2 = new Vertex(vertices[unparsedTriangles[i+1]], unparsedTriangles[i+1], 1, Color.green);
            Vertex vert3 = new Vertex(vertices[unparsedTriangles[i+2]], unparsedTriangles[i+2], 2, Color.blue);
            // if (vert1.pos == vert2.pos | vert2.pos == vert3.pos | vert1.pos == vert3.pos) {
            //     Debug.Log("not good");
            // }

            Triangle tri = new Triangle(
                vert1,
                vert2,
                vert3
            );

            assignColorToTriangle(tri);
            if (!onlyTriangles)
                storeTriCommonalities(tri);
        }
        if (!onlyTriangles)
            DetectNormalDiscontinuities();

        mesh.colors = colors;
        triangleCommons.Clear();
        prebaked = true;
    }

    private void Start() {
        if (meshFilter != null && !meshFilter.mesh.isReadable) {
            Debug.Log("Enable read write on "+gameObject.name+" dumbass");
        }
        Bake();
    }

    private void OnDisable() {
        triangleCommons.Clear();
    }

    private int GetMesh(bool prebakeMode = false) {
        if (meshFilter != null) {
            mesh = prebakeMode ? meshFilter.sharedMesh : meshFilter.mesh;
            return 0;
        }
        if (TryGetComponent(out meshFilter)) {
            mesh = prebakeMode ? meshFilter.sharedMesh : meshFilter.mesh;
            return 0;
        }

        SkinnedMeshRenderer smr;
        if (TryGetComponent(out smr)) {
            mesh = smr.sharedMesh;
            return 1;
        }

        Debug.LogError("Object \""+ gameObject.name+"\" has no meshFilter or skinnedMeshRenderer.");
        return -1;
    }

    [ContextMenu("Bake (runtime only)")]
    private void Bake() {
        triangleCommons.Clear();
        // Debug.Log("---- START ----");
        if (GetMesh() == -1) {
            return;
        }
        
        var vertices = mesh.vertices;
        colors = mesh.colors;

        if (colors.Length == 0) {
            colors = new Color[vertices.Length];
            Array.Fill(colors, Color.black);
        }

        var unparsedTriangles = mesh.triangles;
        // Debug.Log("-- CREATING TRIANGLES --");
        for (int i = 0; i < unparsedTriangles.Length; i += 3) {
            // Debug.Log("Tri "+(i+3)/3+":");
            // Debug.Log(unparsedTriangles[i]+", "+unparsedTriangles[i+1]+", "+unparsedTriangles[i+2]);
            Vertex vert1 = new Vertex(vertices[unparsedTriangles[i]], unparsedTriangles[i], 0, Color.red);
            Vertex vert2 = new Vertex(vertices[unparsedTriangles[i+1]], unparsedTriangles[i+1], 1, Color.green);
            Vertex vert3 = new Vertex(vertices[unparsedTriangles[i+2]], unparsedTriangles[i+2], 2, Color.blue);
            // if (vert1.pos == vert2.pos | vert2.pos == vert3.pos | vert1.pos == vert3.pos) {
            //     Debug.Log("not good");
            // }

            Triangle tri = new Triangle(
                vert1,
                vert2,
                vert3
            );

            assignColorToTriangle(tri);
            if (!onlyTriangles)
                storeTriCommonalities(tri);
        }
        if (!onlyTriangles)
            DetectNormalDiscontinuities();

        mesh.colors = colors;
        triangleCommons.Clear();
    }

    private void Abs(ref Vector3 vec) {
        if (vec.x < 0) {
            vec.x *= -1;
        }

        if (vec.y < 0) {
            vec.y *= -1;
        }

        if (vec.z < 0) {
            vec.z *= -1;
        }
    }

    private Vector3 getNormal(Triangle tri) {
        Vector3 A = tri.vertices[1].pos-tri.vertices[0].pos;
        Vector3 B = tri.vertices[2].pos-tri.vertices[0].pos;

        return Vector3.Cross(A,B).normalized;
    }

    private void addEdgeCommonality((Vector3, Vector3) edge, Triangle tri) {
        var invertedEdge = (edge.Item2, edge.Item1);

        if (!triangleCommons.ContainsKey(edge)) {
            if (!triangleCommons.ContainsKey(invertedEdge)) {
                triangleCommons.Add(edge, new List<Triangle>() { tri });
            }else {
                triangleCommons[invertedEdge].Add(tri);
            }
        }else {
            triangleCommons[edge].Add(tri);
        }
    }

    private void storeTriCommonalities(Triangle tri) {
        addEdgeCommonality(tri.GetEdgePositions(0), tri);
        addEdgeCommonality(tri.GetEdgePositions(1), tri);
        addEdgeCommonality(tri.GetEdgePositions(2), tri);
    }

    private void assignColorToTriangle(Triangle tri) {
        int idx1 = tri.vertices[0].absoluteIndex;
        int idx2 = tri.vertices[1].absoluteIndex;
        int idx3 = tri.vertices[2].absoluteIndex;

        // Debug.Log("     Color idxs: "+idx1+", "+idx2+", "+idx3);
        
        colors[idx1] = Color.red;
        colors[idx2] = Color.green;
        colors[idx3] = Color.blue;
    }

    private Vertex getVertexByPosition(Triangle tri, Vector3 pos, Option<int> blacklistedIndex) {
        for (int i = 0; i <= 2; i++) {
            if (blacklistedIndex.IsSome() && blacklistedIndex.Get() == i) {
                continue;
            }
            if (tri.vertices[i].pos == pos)  {
                return tri.vertices[i];
            }
        }
        Debug.LogWarning("Couldn't find vertex");
        return Vertex.NA;
    }

    private void hideEdge(Triangle tri, (Vertex, Vertex) edge) {
        int other = 3-edge.Item1.relativeIndex-edge.Item2.relativeIndex;
        //Debug.Log("Hiding edge: "+other);

        edge.Item1.hide(tri.vertices[other].color);
        colors[edge.Item1.absoluteIndex] = edge.Item1.color;
        edge.Item2.hide(tri.vertices[other].color);
        colors[edge.Item2.absoluteIndex] = edge.Item2.color;
    }

    private void DetectNormalDiscontinuities() {
        // Debug.Log("- DETECTING NORMAL DISCONTINUITIES -");
        foreach (var key in triangleCommons.Keys) {
            if (triangleCommons[key].Count == 1) {
                continue;
            }
            detectNormalDiscontinuity(key);
        }
    }

    private void detectNormalDiscontinuity((Vector3, Vector3) key) {
        // Debug.Log("!!!!!! Common triangle count: "+triangleCommons[key].Count);

        var tri = triangleCommons[key][0];
        Triangle nextTri = triangleCommons[key][1];

        var normal1 = getNormal(tri);
        var normal2 = getNormal(nextTri);

        // Debug.Log("!! Tri: "+tri.vertices[0].pos);
        // Debug.Log("!! Tri: "+tri.vertices[1].pos);
        // Debug.Log("!! Tri: "+tri.vertices[2].pos);
        // Debug.Log("!! Next Tri:"+nextTri.vertices[0].pos);
        // Debug.Log("!! Next Tri:"+nextTri.vertices[1].pos);
        // Debug.Log("!! Next Tri:"+nextTri.vertices[2].pos);


        //case 1:
        // Debug.Log(" Normal1: "+ normal1);
        // Debug.Log(" Normal1: "+ normal2);

        var diff = normal1-normal2;
        Abs(ref diff);

        if (diff.x < sensitivity && diff.y < sensitivity && diff.z < sensitivity) { // normal continuity
            // Debug.Log("[-[-[-[-[ CONTINUITY FOUND: ");
            Vertex Avert1 = getVertexByPosition(tri, key.Item1, Option<int>.None());
            Vertex Avert2 = getVertexByPosition(tri, key.Item2, new Option<int>(Avert1.relativeIndex));
            hideEdge(tri, (Avert1, Avert2));
            Vertex Bvert1 = getVertexByPosition(nextTri, key.Item1, Option<int>.None());
            Vertex Bvert2 = getVertexByPosition(nextTri, key.Item2, new Option<int>(Bvert1.relativeIndex));
            hideEdge(nextTri, (Bvert1, Bvert2)); 
        }
    }
}