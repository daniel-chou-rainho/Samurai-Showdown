using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.InputSystem;

public class GameMenuManager : MonoBehaviour
{
    public Transform head;
    public float spawnDistance = 2;
    public GameObject menu;
    public InputActionProperty showButton;

    private UnityEngine.XR.InputDevice leftHandDevice;
    private bool menuButtonPressed = false;


    private void Awake()
    {
        List<UnityEngine.XR.InputDevice> devices = new List<UnityEngine.XR.InputDevice>();
        InputDevices.GetDevices(devices);
        foreach (var device in devices)
        {
            if ((device.characteristics & InputDeviceCharacteristics.Left) == InputDeviceCharacteristics.Left)
            {
                leftHandDevice = device;
                break;
            }
        }
    }

    void Update()
    {
        menuButtonPressed = false;
        
        if(showButton.action.WasPressedThisFrame())
        {
            if (!menuButtonPressed)
            {
                menuButtonPressed = true;

                // TimeStop
                if (!menu.activeSelf){ Time.timeScale = 0f; }
                else { Time.timeScale = 1f; }

                menu.SetActive(!menu.activeSelf);

                menu.transform.position = head.position
                    + new Vector3(head.forward.x, 0, head.forward.z).normalized * spawnDistance;
            }
            else if (menuButtonPressed)
            {
                menuButtonPressed = false;
            } 
        }

        menu.transform.LookAt(
            new Vector3(head.position.x, menu.transform.position.y, head.position.z)
        );

        menu.transform.forward *= -1;
    }
}
