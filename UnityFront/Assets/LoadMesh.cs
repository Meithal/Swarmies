using System;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

public class LoadMesh : MonoBehaviour
{
    
    [StructLayout(LayoutKind.Sequential)]
    public struct FloatArray
    {
        public float x, y, z;
    }

    //public Button loadMeshButton;
    [DllImport("UnityGlue")]
    private static extern void LoadLevelMesh(char[] levelName, out float[][] vertices);
    
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

        var vertices = new float[vc][];
        
        LoadLevelMesh("test_topo".ToCharArray(), out vertices);
        
        Debug.Log($"verticles {vertices.Length}");
        foreach (var vertex in vertices)
        {
            Debug.Log(vertex);
        }
    }
}
