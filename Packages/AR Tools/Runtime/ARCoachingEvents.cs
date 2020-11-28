using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.ARFoundation;
#if UNITY_IOS && !UNITY_EDITOR
using UnityEngine.XR.ARKit;
#endif

public class ARCoachingEvents : MonoBehaviour
{
    public bool debugLog;

    public UnityEvent onARCoachingBegin;
    public UnityEvent onARCoachingEnd;

    ARSession arSession;
    // [ReadOnly]
    [SerializeField]
    bool arCoachingActive;
    bool arCoachingActiveLast;
    #if UNITY_IOS && !UNITY_EDITOR
    ARKitSessionSubsystem arKitSessionSubsystem;
    #endif

    void Awake()
    {
        arSession = FindObjectOfType<ARSession>();

        #if UNITY_IOS && !UNITY_EDITOR
        if (arSession.subsystem is ARKitSessionSubsystem)
            arKitSessionSubsystem = arSession.subsystem as ARKitSessionSubsystem;
        #endif
    }

    void OnARCoachingBegin()
    {
        if (debugLog)
            Debug.Log("invoking: onARCoachingBegin");

        onARCoachingBegin.Invoke();
    }

    void OnARCoachingEnd()
    {
        if (debugLog)
            Debug.Log("invoking: onARCoachingEnd");

        onARCoachingEnd.Invoke();
    }

    void Update()
    {
        #if UNITY_IOS && !UNITY_EDITOR
        arCoachingActive = arKitSessionSubsystem.coachingActive;
        #endif

        if (arCoachingActive != arCoachingActiveLast)
        {
            if (arCoachingActive)
                OnARCoachingBegin();
            else
                OnARCoachingEnd();
        }

        arCoachingActiveLast = arCoachingActive;
    }
}
