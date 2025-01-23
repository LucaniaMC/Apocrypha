using UnityEngine;
using Cinemachine;

public class CameraManager : MonoBehaviour
{
    public CinemachineVirtualCamera normalCamera;
    public CinemachineVirtualCamera highCamera;
    public CinemachineVirtualCamera lowCamera;

    int activePriority = 10;
    int inactivePriority = 5;

    public PlayerController controller;


    void Update()
    {
        if (controller.lookingUp) 
        {
            SetHighCamera();
        }
        else if (controller.lookingDown) 
        {
            SetLowCamera();
        }
        else if (!controller.lookingUp && !controller.lookingDown) 
        {
            SetNormalCamera();
        }
    }

    void SetHighCamera() 
    {
        normalCamera.Priority = inactivePriority;
        highCamera.Priority = activePriority;
        lowCamera.Priority = inactivePriority;
    }


    void SetLowCamera() 
    {
        normalCamera.Priority = inactivePriority;
        highCamera.Priority = inactivePriority;
        lowCamera.Priority = activePriority;
    }


    void SetNormalCamera() 
    {
        normalCamera.Priority = activePriority;
        highCamera.Priority = inactivePriority;
        lowCamera.Priority = inactivePriority;
    }
}
