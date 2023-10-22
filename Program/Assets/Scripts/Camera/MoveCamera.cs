using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.Serialization;
using TouchPhase = UnityEngine.TouchPhase;

public class MoveCamera : MonoBehaviour
{

    public float scrollSpeed = 1f;
    public MeshFilter clampTo;
    private Camera _cam;
    private Vector2 _touch;
    private Gamepad _gamepad;
    private Vector3 _topLeftPos;
    private Vector3 _bottomRightPos;

    private void OnEnable()
    {
        _gamepad = Gamepad.current;

    }

    private void OnDisable()
    {
        _gamepad = null;
    }

    // Start is called before the first frame update
    void Start()
    {
        _cam = GetComponent<Camera>();
        // Debug.Log(_cam.pixelRect);
        // Debug.Log(clampTo.transform.position);//.GetComponent<MeshFilter>().GetComponent<Mesh>());
        // Debug.Log(clampTo.mesh.bounds.size);//.GetComponent<MeshFilter>().GetComponent<Mesh>());
        // Debug.Log(clampTo.mesh.bounds.min);//.GetComponent<MeshFilter>().GetComponent<Mesh>());
        // Debug.Log(clampTo.mesh.bounds.center);//.GetComponent<MeshFilter>().GetComponent<Mesh>());
        ComputeCameraBounds();
        Debug.Log(_topLeftPos);
        Debug.Log(_bottomRightPos);
    }

    void ComputeCameraBounds()
    {
        Mesh mesh = clampTo.mesh;
        Transform transform = clampTo.transform;
        Vector3 position = transform.position;
        Vector3 scale = transform.lossyScale;
        var meshWidth = mesh.bounds.size.x;
        var meshHeight = mesh.bounds.size.z;
        _topLeftPos = new Vector3(
            x: (meshWidth / 2f) * scale.x * -1, 
            y: 0,
            z: (meshHeight / 2f) * scale.z
            );
        _bottomRightPos = new Vector3(
            x: (meshWidth / 2f) * scale.x,
            y: 0,
            z: (meshHeight / 2f) * scale.z * -1
            );
        //Debug.Log(transform.TransformPoint(_topLeftPos));
        //Debug.Log(transform.TransformPoint(_bottomRightPos));
        //_bottomRightPos.
    }
    bool CameraIsInBounds()
    {
        return true;
    }
    
    // Update is called once per frame
    void Update()
    {
        _gamepad = Gamepad.current;
        
        var x = 0f;
        var y = 0f;
        Vector3 pos = _cam.transform.position;
        // Debug.Log($"Cam: {pos} plane: {clampTo.mesh.bounds.max} Ray: {hit}"); 
        // Debug.Log(_cam.transform.position);
        Debug.DrawLine(_topLeftPos, _bottomRightPos, Color.red);
        //Debug.DrawLine(Vector3.left, Vector3.right, Color.blue);

        #region Gamepad

        if (_gamepad != null)
        {
            var sx = _gamepad.leftStick.x.value;
            var sy = _gamepad.leftStick.y.value;
            if (sx != 0 || sy != 0)
            {
                x = sx;
                y = sy;
            }
        }

        #endregion
        
        #region mouse
        Vector2Control mp = Mouse.current.position;
        Rect rect = _cam.pixelRect;

        if (mp.x.value is >= 0 and < 5)
        {
            x = -scrollSpeed;
        }
        else if(mp.x.value  <= rect.width && mp.x.value > rect.width - 5)
        {
            x = scrollSpeed;
        }

        if (mp.y.value is >= 0 and < 5)
        {
            y = -scrollSpeed;
        }
        else if (mp.y.value <= rect.height && mp.y.value > rect.height - 15)
        {
            y = scrollSpeed;
        }
        #endregion
        
        #region keyboard
        if (Keyboard.current.leftArrowKey.isPressed)
        {
            x = -scrollSpeed;
        }
        else if (Keyboard.current.rightArrowKey.isPressed)
        {
            x = scrollSpeed;
        }

        if (Keyboard.current.upArrowKey.isPressed)
        {
            y = scrollSpeed;
        }
        else if (Keyboard.current.downArrowKey.isPressed)
        {
            y = -scrollSpeed;
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

        if (x == 0 && y == 0) return;
        
        pos.x += x;
        pos.z += y;
        pos.x = Mathf.Clamp(pos.x, _topLeftPos.x, _bottomRightPos.x);
        pos.z = Mathf.Clamp(pos.z, _bottomRightPos.z, _topLeftPos.z);
        
        _cam.transform.position = pos;
        Debug.Log(pos);
    }

    private void LateUpdate()
    {
        Vector3 cameraPosition = transform.position;

        // Clamp the camera's height above the mesh
        //cameraPosition.x = Mathf.Clamp(
        //    cameraPosition.x, clampTo.transform.position.y + minHeight, targetMesh.position.y + maxHeight);

        // Set the camera's new position
        transform.position = cameraPosition;
    }
}
