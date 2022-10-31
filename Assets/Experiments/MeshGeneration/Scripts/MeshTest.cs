using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshTest : MonoBehaviour
{
    private const int quadSize = 100;

    void Start()
    {
        Mesh mesh = CreateQuad(quadSize);

        GetComponent<MeshFilter>().mesh = mesh;
    }

    private static Mesh CreateQuad(int quadSideSize)
    {
        Mesh mesh = new Mesh();

        Vector3[] vertices = new Vector3[4];
        Vector2[] uv = new Vector2[4];
        int[] triangles = new int[6];

        //first triangle
        vertices[0] = new Vector3(0, 0);
        vertices[1] = new Vector3(0, quadSideSize);
        vertices[2] = new Vector3(quadSideSize, quadSideSize);

        vertices[3] = new Vector3(quadSideSize, 0);

        uv[0] = new Vector2(0, 0);
        uv[1] = new Vector2(0, 1);
        uv[2] = new Vector2(1, 1);

        uv[3] = new Vector3(1, 0);

        triangles[0] = 0;
        triangles[1] = 1;
        triangles[2] = 2;

        triangles[3] = 0;
        triangles[4] = 2;
        triangles[5] = 3;

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
        return mesh;
    }

}
