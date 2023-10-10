using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    private Camera cam;
    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponent<Camera>();
        Debug.Log(cam.pixelRect);
    }

    // Update is called once per frame
    void Update()
    {
        var mp = Input.mousePosition;

        Rect rect = cam.pixelRect;

        var x = 0;
        var y = 0;
        Vector3 pos = cam.transform.position;

        if(mp.x >= 0 && mp.x < 5)
        {
            x = -1;
        } else if(mp.x <= rect.width && mp.x > rect.width - 5)
        {
            x = 1;
        }

        if (mp.y >= 0 && mp.y < 5)
        {
            y = -1;
        }
        else if (mp.y <= rect.height && mp.y > rect.height - 15)
        {
            y = 1;
        }

        if (x != 0 || y != 0)
        {
            pos.x += x;
            pos.z += y;
            cam.transform.position = pos;
        }
    }
}
