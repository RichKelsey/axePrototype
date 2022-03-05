using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraHandler : MonoBehaviour
{
    public Cinemachine.CinemachineFreeLook standardCam;
    public Cinemachine.CinemachineVirtualCamera aimCam;

    public bool aiming;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        if (aiming)
        {
            aimCam.Priority = 1;
            standardCam.Priority = 0;

        }
        else
        {
            aimCam.Priority = 0;
            standardCam.Priority = 1;
        }

    }
}
