using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
#if UNITY_IOS
using UnityEngine.iOS;
#endif

public class ARMeshSupportedEvents : MonoBehaviour
{
    public bool debugLogInvoke;
    public bool debugLogDeviceInfo;
    public UnityEvent meshSupportedDeviceAwake;
    public UnityEvent meshUnsupportedDeviceAwake;

    void Awake()
    {
        #if UNITY_IOS
        string generation = Device.generation.ToString();

        if (debugLogDeviceInfo)
            Debug.Log("deviceGeneration: " + generation);

        if(generation.ContainsAll("iPadPro", "2Gen") || generation.ContainsAll("iPad","Unknown"))
        {
            if (debugLogInvoke)
                Debug.Log("invoking mesh Supported DeviceAwake()");
            meshSupportedDeviceAwake.Invoke();
        }
        else
        {
            meshUnsupportedDeviceAwake.Invoke();
            if (debugLogInvoke)
                Debug.Log("invoking mesh Unsupported DeviceAwake()");
        }
        #endif
    }
}
