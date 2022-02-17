using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct PathInfo
{
    public int MaxPath;
    public int CurrentPath;
}

public class PlayerMovement : MonoBehaviour
{
    public float _velocityX;
    public SO.ControlBindings _controlBindings;
    public PathInfo _pathInfo;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        JumpToPath();
    }

    void JumpToPath()
    {
        if (Input.GetButtonDown(_controlBindings.yAxis) && 
            Input.GetAxis(_controlBindings.yAxis) > 0 &&
            _pathInfo.CurrentPath < _pathInfo.MaxPath - 1)
        {
            _pathInfo.CurrentPath++;
            this.gameObject.transform.position = new Vector3(0, this.gameObject.transform.position.y, (_pathInfo.CurrentPath * 4) + 0.5f);
        }
        else if (Input.GetButtonDown(_controlBindings.yAxis) && 
            Input.GetAxis(_controlBindings.yAxis) < 0 &&
            _pathInfo.CurrentPath > 0)
        {
            _pathInfo.CurrentPath--;
            this.gameObject.transform.position = new Vector3(0, this.gameObject.transform.position.y, (_pathInfo.CurrentPath * 4) + 0.5f);
        }      
    }
}
