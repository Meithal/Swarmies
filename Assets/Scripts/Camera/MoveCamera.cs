using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{

    public float ScrollSpeed = 1f;
    private Camera _cam;
    private Vector2 _touch;

    // Start is called before the first frame update
    void Start()
    {
        _cam = GetComponent<Camera>();
        Debug.Log(_cam.pixelRect);
    }

    // Update is called once per frame
    void Update()
    {

        Rect rect = _cam.pixelRect;

        var x = 0f;
        var y = 0f;
        Vector3 pos = _cam.transform.position;

        #region mouse
        var mp = Input.mousePosition;

        if (mp.x >= 0 && mp.x < 5)
        {
            x = -ScrollSpeed;
        }
        else if(mp.x <= rect.width && mp.x > rect.width - 5)
        {
            x = ScrollSpeed;
        }

        if (mp.y >= 0 && mp.y < 5)
        {
            y = -ScrollSpeed;
        }
        else if (mp.y <= rect.height && mp.y > rect.height - 15)
        {
            y = ScrollSpeed;
        }
        #endregion
        
        #region keyboard
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            x = -ScrollSpeed;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            x = ScrollSpeed;
        }

        if (Input.GetKey(KeyCode.UpArrow))
        {
            y = ScrollSpeed;
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            y = -ScrollSpeed;
        }
        #endregion

        #region TouchScreen
        if(Input.touchSupported && Input.touchCount == 2)
        {
            var touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                this._touch = touch.position;
            }
            else if(touch.phase == TouchPhase.Moved)
            {
                x = touch.position.x - _touch.x;
                y = touch.position.y - _touch.y;
            }
            
        }
        #endregion

        #region LaptopTrackPad

        var dtp = Input.mouseScrollDelta;
        x = -dtp.x;
        y = dtp.y;
        #endregion
        
        if (x != 0 || y != 0)
        {
            pos.x += x;
            pos.z += y;
            _cam.transform.position = pos;
        }
    }
}