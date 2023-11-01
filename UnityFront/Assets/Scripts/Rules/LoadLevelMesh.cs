using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class LoadLevelMesh : MonoBehaviour
{
    [DllImport("UnityGlue")]
    private static extern float SayHello();
    
    // Start is called before the first frame update
    void Start()
    {
        float ext = SayHello();
        Debug.Log(ext);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
