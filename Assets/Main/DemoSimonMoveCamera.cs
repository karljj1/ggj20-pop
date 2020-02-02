using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoSimonMoveCamera : MonoBehaviour
{
    public float CameraSpeed = 5;
    void Update()
    {
        if (Input.GetKey(KeyCode.A))
            transform.position += new Vector3(-CameraSpeed * Time.deltaTime, 0, 0);

        if (Input.GetKey(KeyCode.D))
            transform.position += new Vector3(CameraSpeed * Time.deltaTime, 0, 0);

        if (Input.GetKey(KeyCode.W))
            transform.position += new Vector3(0, Time.deltaTime * CameraSpeed, 0);

        if (Input.GetKey(KeyCode.S))
            transform.position += new Vector3(0, Time.deltaTime* -CameraSpeed, 0);

    }
}
