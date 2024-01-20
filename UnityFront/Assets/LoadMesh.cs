using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

public class LoadMesh : MonoBehaviour
{

    public GameObject target;
    
    //public Button loadMeshButton;
    [DllImport("UnityGlue")]
    private static extern void LoadLevelMesh(char[] levelName, float[] vertices, float[] norms, float[] uvs, int[] tris);
    
    [DllImport("UnityGlue")]
    private static extern int MeshVerticesNumber(char[] levelName);
    [DllImport("UnityGlue")]
    private static extern int MeshNormalsNumber(char[] levelName);
    [DllImport("UnityGlue")]
    private static extern int MeshUVNumber(char[] levelName);
    [DllImport("UnityGlue")]
    private static extern int MeshTrianglesNumber(char[] levelName);
    
    [DllImport("UnityGlue")]
    private static extern float SayHello();
    
    public void OnButtonClick()
    {
        Debug.Log("Click");
        //LoadLevelMesh("test_topo".ToCharArray(), );
        //SayHello();
        Debug.Log( MeshVerticesNumber("test_topo".ToCharArray()));
        
        int vc = MeshVerticesNumber("test_topo".ToCharArray());
        int nc = MeshNormalsNumber("test_topo".ToCharArray());
        int uvc = MeshUVNumber("test_topo".ToCharArray());
        int tc = MeshTrianglesNumber("test_topo".ToCharArray());

        var vertices = new float[vc*3];
        var norms = new float[nc*3];
        var uvs = new float[uvc * 2];
        var tris = new int[tc];
        
        LoadLevelMesh("test_topo".ToCharArray(), vertices, norms, uvs, tris);

        var mesh = new Mesh
        {
            name = "test_topo",
        };
        //mesh.triangles = new int[tc];
        var vertices_p = new Vector3[vc];
        for (int i = 0; i < vertices.Length / 3; i++)
        {
            vertices_p[i] = new Vector3(vertices[i * 3 + 0], vertices[i * 3 + 1], vertices[i * 3 + 2]);
        }

        var normals_p = new Vector3[nc];
        for (int i = 0; i < norms.Length / 3; i++)
        {
            normals_p[i] = new Vector3(norms[i * 3 + 0], norms[i * 3 + 1], norms[i * 3 + 2]);
        }

        
        mesh.Clear();
        mesh.vertices = vertices_p;
        mesh.normals = normals_p;
        mesh.triangles = tris;

        GameObject go = new GameObject("test_topo");
        var meshFilter = go.AddComponent<MeshFilter>();
        meshFilter.mesh = mesh;
        
        MeshRenderer meshRenderer = go.AddComponent<MeshRenderer>(); 
        
        
        // Assign the default material to the MeshRenderer
        meshRenderer.material = new Material(Shader.Find("Diffuse"));

        go.transform.parent = target.transform;
        go.transform.localScale *= 10;
         
        Debug.Log($"verticles {vertices.Length} {norms.Length}  {tris.Length}");
        // foreach (var vertex in vertices)
        // {
        //     Debug.Log(vertex);
        // }
    }
}
