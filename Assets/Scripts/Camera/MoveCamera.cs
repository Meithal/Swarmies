using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using TouchPhase = UnityEngine.TouchPhase;

public class MoveCamera : MonoBehaviour
{

    public float ScrollSpeed = 1f;
    private Camera _cam;
    private Vector2 _touch;
    //private Gamepad

    // Start is called before the first frame update
    void Start()
    {
        _cam = GetComponent<Camera>();
        Debug.Log(_cam.pixelRect);
    }

    // Update is called once per frame
    void Update()
    {
        var js = Input.GetJoystickNames();
        //Debug.Log(js.Length);

        if (js.Length > 0)
        {
            //Debug.Log(js.GetValue(0));
        }

        Rect rect = _cam.pixelRect;

        var x = 0f;
        var y = 0f;
        Vector3 pos = _cam.transform.position;

        #region mouse
        Vector2Control mp = Mouse.current.position;

        if (mp.x.value >= 0 && mp.x.value < 5)
        {
            x = -ScrollSpeed;
        }
        else if(mp.x.value <= rect.width && mp.x.value > rect.width - 5)
        {
            x = ScrollSpeed;
        }

        if (mp.y.value >= 0 && mp.y.value < 5)
        {
            y = -ScrollSpeed;
        }
        else if (mp.y.value <= rect.height && mp.y.value > rect.height - 15)
        {
            y = ScrollSpeed;
        }
        #endregion
        
        #region keyboard
        if (Keyboard.current.leftArrowKey.isPressed)
        {
            x = -ScrollSpeed;
        }
        else if (Keyboard.current.rightArrowKey.isPressed)
        {
            x = ScrollSpeed;
        }

        if (Keyboard.current.upArrowKey.isPressed)
        {
            y = ScrollSpeed;
        }
        else if (Keyboard.current.downArrowKey.isPressed)
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
        if (dtp.x != 0 || dtp.y != 0)
        {
            x = -dtp.x;
            y = dtp.y;
        }

        #endregion
        
        if (x != 0 || y != 0)
        {
            pos.x += x;
            pos.z += y;
            _cam.transform.position = pos;
        }
    }
}
