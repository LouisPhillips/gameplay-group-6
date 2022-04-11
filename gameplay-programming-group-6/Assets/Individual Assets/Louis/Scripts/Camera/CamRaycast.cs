using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public class CamRaycast : MonoBehaviour
{
    Ray RayOrigin;
    RaycastHit HitInfo;

    public CinemachineFreeLook cam;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void LateUpdate()
    {
        {
            RayOrigin = Camera.main.ViewportPointToRay(new Vector3(0, 0, 0));
            if (Physics.Raycast(cam.VirtualCameraGameObject.transform.position, cam.VirtualCameraGameObject.transform.forward, out HitInfo, 100.0f))
            {
                Debug.DrawRay(cam.VirtualCameraGameObject.transform.position, cam.VirtualCameraGameObject.transform.forward * 100.0f, Color.yellow);
            }
        }

    }
 
}
