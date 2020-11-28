using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.Events;
using System;

[Serializable]
public class TrackingStateEvent : UnityEvent<NotTrackingReason>{}

[RequireComponent(typeof(ARSession))]
public class ARTrackingQualityEventRelay : MonoBehaviour
{
    public TrackingStateEvent trackingStateNone;
    public TrackingStateEvent trackingStateLimited;
    public UnityEvent trackingStateTracking;

    public bool debugLog = false;

    ARSession arSession;
    TrackingState trackingState;
    TrackingState trackingStateLast;
    NotTrackingReason notTrackingReason;
    NotTrackingReason notTrackingReasonLast;

    void Awake()
    {
        if (Application.isEditor)
            debugLog = false;
            
        arSession = GetComponent<ARSession>();
        if (arSession.subsystem != null)
        {
            trackingState = arSession.subsystem.trackingState;
            trackingStateLast = arSession.subsystem.trackingState;
            notTrackingReason = arSession.subsystem.notTrackingReason;
            notTrackingReasonLast = arSession.subsystem.notTrackingReason;
        }
        if (debugLog)
            Debug.Log("tracking state on awake is: " + trackingState);
    }

    void Update()
    {
        if (arSession == null)
            return;
            
        if (arSession.subsystem == null)
            return;     
        
        trackingState = arSession.subsystem.trackingState;
        notTrackingReason = arSession.subsystem.notTrackingReason;

        // if tracking state changed
        if (trackingState != trackingStateLast)
        {
            switch(trackingState)
            {
                case TrackingState.None:
                {
                    if (debugLog)
                        Debug.Log("tracking state changed to: " + trackingState + "\nwith reason: " + notTrackingReason);
                    
                    trackingStateNone.Invoke(notTrackingReason);
                }
                break;
                case TrackingState.Limited:
                {
                    if (debugLog)
                        Debug.Log("tracking state changed to: " + trackingState + "\nwith reason: " + notTrackingReason);

                        trackingStateLimited.Invoke(notTrackingReason);
                }
                break;
                case TrackingState.Tracking:
                {
                    if (debugLog)
                        Debug.Log("tracking state changed to: " + trackingState);

                    trackingStateTracking.Invoke();
                }
                break;
            }
        }
        // else if already not tracking, and not tracking reason changes
        else if (trackingState == TrackingState.Limited || trackingState == TrackingState.None)
        {
            if (notTrackingReason != notTrackingReasonLast)
            {
                switch(notTrackingReason)
                {
                    case NotTrackingReason.ExcessiveMotion:
                    {
                        if (debugLog)
                            Debug.Log("not tracking reason changed to: " + notTrackingReason);
                    }
                    break;
                    case NotTrackingReason.Initializing:
                    {
                        if (debugLog)
                            Debug.Log("not tracking reason changed to: " + notTrackingReason);
                    }
                    break;
                    case NotTrackingReason.InsufficientFeatures:
                    {
                        if (debugLog)
                            Debug.Log("not tracking reason changed to: " + notTrackingReason);
                    }
                    break;
                    case NotTrackingReason.InsufficientLight:
                    {
                        if (debugLog)
                            Debug.Log("not tracking reason changed to: " + notTrackingReason);
                    }
                    break;
                    case NotTrackingReason.None:
                    {
                        if (debugLog)
                            Debug.Log("not tracking reason changed to: " + notTrackingReason);
                    }
                    break;
                    case NotTrackingReason.Relocalizing:
                    {
                        if (debugLog)
                            Debug.Log("not tracking reason changed to: " + notTrackingReason);
                    }
                    break;
                    case NotTrackingReason.Unsupported:
                    {
                        if (debugLog)
                            Debug.Log("not tracking reason changed to: " + notTrackingReason);
                    }
                    break;
                }
            }
        }

        trackingStateLast = arSession.subsystem.trackingState;
        notTrackingReasonLast = arSession.subsystem.notTrackingReason;
    }

}
