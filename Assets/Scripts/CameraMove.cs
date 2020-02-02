using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed = 0.125f;
    public Vector3 offset;
    public Vector2 offsetMinMax;
    CharacterController charCtrl;

    private void Start()
    {
        charCtrl = FindObjectOfType<CharacterController>();
    }

    void FixedUpdate()
    {
        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothPosition;
    }


    //void PanCamera()
    //{
    //    Mathf.Clamp(offset.x, offsetMinMax.x, offsetMinMax.y);

    //    if (charCtrl.m_MoveDir > 0)
    //    {
    //        offset.x++;
    //    }
    //}

}
