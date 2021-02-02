using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.ARFoundation;

namespace Edwon.ARTools
{
    public class ARMeshSupportedEvents : MonoBehaviour
    {
        public bool debugLog;
        public UnityEvent meshSupportedDeviceAwake;
        public UnityEvent meshUnsupportedDeviceAwake;
        UnityEngine.XR.ARFoundation.ARMeshManager arMeshManager;
        bool arSessionReady;

        void Awake()
        {
            // Debug.Log("ARMeshSupportedEvents this script currently broken due to issues with AR Foundation Remote");
            arSessionReady = false;

            arMeshManager = FindObjectOfType<ARMeshManager>();

            if (arMeshManager != null)
                StartCoroutine(Init());
            else
                Debug.Log("ARMeshManager is null on ARMeshSupportedEvents");
        }

        IEnumerator Init()
        {
            while(!arSessionReady)
            {
                if (ARSession.state == ARSessionState.Ready)
                {
                    arSessionReady = true;
                    // if (arMeshManager.subsystem != null)
                    // {
                    //     if (debugLog)
                    //         Debug.Log("invoking mesh Supported DeviceAwake()");
                    //     meshSupportedDeviceAwake.Invoke();
                    // }
                    // else
                    // {
                    //     if (debugLog)
                    //         Debug.Log("invoking mesh Unsupported DeviceAwake()");
                    //     meshUnsupportedDeviceAwake.Invoke();
                    // }
                }
                yield return null;
            }
        }
    }
}