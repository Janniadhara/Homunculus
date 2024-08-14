using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraZoom : MonoBehaviour
{
    [SerializeField] CinemachineVirtualCamera virtualCamera;
    private float camDistance;
    // Start is called before the first frame update
    void Start()
    {
        camDistance = virtualCamera.GetCinemachineComponent<Cinemachine3rdPersonFollow>().CameraDistance;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            camDistance -= Time.deltaTime*16;
            if (camDistance < 1.5f)
            {
                camDistance = 1.5f;
            }
            virtualCamera.GetCinemachineComponent<Cinemachine3rdPersonFollow>().CameraDistance = camDistance;
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            camDistance += Time.deltaTime * 16;
            if (camDistance > 6f)
            {
                camDistance = 6f;
            }
            virtualCamera.GetCinemachineComponent<Cinemachine3rdPersonFollow>().CameraDistance = camDistance;
        }
    }
}
