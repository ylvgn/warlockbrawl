using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRoam : MonoBehaviour
{
    float cameraMoveSpeed = 20;
    float screenBorder = 10;

    void Update()
    {
        Vector3 offset = Vector3.zero;

        // up
        if (Input.mousePosition.y >= Screen.height - screenBorder)
        {
            offset.z -= cameraMoveSpeed * Time.deltaTime;
        }

        // down
        else if (Input.mousePosition.y <= screenBorder)
        {
            offset.z += cameraMoveSpeed * Time.deltaTime;
        }

        // left
        if (Input.mousePosition.x <= screenBorder)
        {
            offset.x += cameraMoveSpeed * Time.deltaTime;
        }

        // right
        else if (Input.mousePosition.x >= Screen.width - screenBorder)
        {
            offset.x -= cameraMoveSpeed * Time.deltaTime;
        }

        transform.position += offset;
    }
}
