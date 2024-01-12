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
    private static extern void LoadLevelMesh(char[] levelName, int size, float[] vertices);
    
    [DllImport("UnityGlue")]
    private static extern int MeshVerticesNumber(char[] levelName);
    
    [DllImport("UnityGlue")]
    private static extern float SayHello();
    
    // Start is called before the first frame update
    void Start()
    {
        //loadMeshButton.onClick.AddListener((() => Debug.Log("Click")));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnButtonClick()
    {
        Debug.Log("Click");
        //LoadLevelMesh("test_topo".ToCharArray(), );
        //SayHello();
        Debug.Log( MeshVerticesNumber("test_topo".ToCharArray()));
        
        int vc = MeshVerticesNumber("test_topo".ToCharArray());

        var vertices = new float[vc*3];
        
        LoadLevelMesh("test_topo".ToCharArray(), vc, vertices);

        Mesh mesh = new Mesh();
        mesh.name = "test_topo";

        for (int i = 0; i < vertices.Length / 3; i++)
        {
            mesh.vertices.Append(new Vector3(vertices[i * 3 + 0], vertices[i * 3 + 1], vertices[i * 3 + 2]));
        }

        MeshFilter meshFilter = target.AddComponent<MeshFilter>();
        meshFilter.mesh = mesh;
        
        
        Debug.Log($"verticles {vertices.Length}");
        foreach (var vertex in vertices)
        {
            Debug.Log(vertex);
        }
    }
}
