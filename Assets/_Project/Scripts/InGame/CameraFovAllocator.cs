using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFovAllocator : MonoBehaviour
{
    void Start()
    {
        int width = Screen.width;
        int height = Screen.height;
        
        float ratio = (float)width / height;
        ratio = Mathf.Clamp(ratio,1.25f, 2.3f);
        ratio -= 1.25f;
        ratio /= 1.05f;
        ratio = Mathf.Clamp01(ratio);
        ratio = ratio * (-1) + 1;
        float fov = 60 + ratio * 27;
        GetComponent<Camera>().fieldOfView = fov;
    }
}
