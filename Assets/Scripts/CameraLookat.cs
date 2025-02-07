using UnityEngine;
using Cinemachine;

public class CameraLookat : MonoBehaviour
{
    public CinemachineVirtualCamera normalCamera;
    public CinemachineVirtualCamera highCamera;
    public CinemachineVirtualCamera lowCamera;

    //Priorities are used to set which camera is currently active
    readonly int activePriority = 10;
    readonly int inactivePriority = 5;


    public void SetHighCamera() 
    {
        normalCamera.Priority = inactivePriority;
        highCamera.Priority = activePriority;
        lowCamera.Priority = inactivePriority;
    }


    public void SetLowCamera() 
    {
        normalCamera.Priority = inactivePriority;
        highCamera.Priority = inactivePriority;
        lowCamera.Priority = activePriority;
    }


    public void SetNormalCamera() 
    {
        normalCamera.Priority = activePriority;
        highCamera.Priority = inactivePriority;
        lowCamera.Priority = inactivePriority;
    }
}
