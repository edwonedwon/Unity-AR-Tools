using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(RigScaler))]
public class ARDraggableRagdoll : MonoBehaviour, IARDraggableObject
{
    public bool debugLog;
    public bool debugDraw;
    [ReadOnly]
    [SerializeField]
    bool draggingEnabled;
    public LayerMask raycastLayerMask;
    GameObject player;
    Camera playerCamera;
    public Rigidbody rigidbodyToDrag;
    public Transform feetPosition;
    [ReadOnly]
    [SerializeField]
    float distanceToFeet;
    public Transform ragdollParent;
    Vector3 targetPosition;
    Quaternion targetRotation;
    ResetableRigidbody[] resetableRigidbodies;
    List<Rigidbody> rigidbodies;
    public float moveTime;  
    public float moveMaxSpeed;
    public float rotateTime;
    Vector3 velocity = Vector3.zero;
    public float distanceFromCamera = 1f;
    RigScaler rigScaler;
    
    void Awake()
    {
        draggingEnabled = true;
        player = GameObject.FindWithTag("Player");
        rigScaler = GetComponent<RigScaler>();
        playerCamera = player.GetComponentInChildren<Camera>();
        resetableRigidbodies = transform.GetComponentsInChildren<ResetableRigidbody>();
        rigidbodies = transform.GetComponentsInChildren<Rigidbody>().ToList();
        distanceToFeet = Vector3.Distance(rigidbodyToDrag.transform.position, feetPosition.position);
        distanceToFeet *= ragdollParent.lossyScale.y;
    }

    [InspectorButton("DisableDragging")]
    public bool disableDragging;
    public void DisableDragging()
    {
        OnDragEnd(Vector2.zero);
        draggingEnabled = false;
    }

    public void OnDragBegin(Vector2 screenPos)
    {
        if (!draggingEnabled)
            return;

        if (debugLog)
            Debug.Log(gameObject.name + " OnDragBegin");

        rigidbodyToDrag.isKinematic = true;
    }

    public void OnDragUpdate(Vector2 screenPos)
    {
        if (!draggingEnabled)
            return;

        if (debugLog)
            Debug.Log(gameObject.name + " OnDragUpdate");

        // RAYCAST
        Ray ray = playerCamera.ScreenPointToRay(screenPos);

        if (debugDraw)
            Debug.DrawRay(ray.origin, ray.direction, Color.red, .001f);

        if (Physics.Raycast(ray, out RaycastHit hit, 10000000, raycastLayerMask))
        {
            // UPDATE POSITION
            targetPosition = new Vector3(
                hit.point.x, 
                hit.point.y + (distanceToFeet * ragdollParent.lossyScale.y), 
                hit.point.z);
       
            if (debugDraw)
                Debug.DrawLine(hit.point, targetPosition, Color.magenta);
        }
        
        // UPDATE ROTATION
        Vector3 forward = Vector3.ProjectOnPlane(playerCamera.transform.position - hit.point, Vector3.up);
        targetRotation = Quaternion.LookRotation(forward, Vector3.up);

        if (!rigScaler.tweening)
        {
            // LERP
            Vector3 targetPositionSmooth = Vector3.SmoothDamp(rigidbodyToDrag.transform.position, targetPosition, ref velocity, moveTime, moveMaxSpeed);
            Quaternion targetRotationSmooth = Quaternion.RotateTowards(rigidbodyToDrag.transform.rotation, targetRotation, rotateTime);
            // MOVE rigidbodyToDrag
            rigidbodyToDrag.MovePosition(targetPositionSmooth);
            rigidbodyToDrag.MoveRotation(targetRotationSmooth);
        }
        else
        {
            // LERP
            Vector3 targetPositionSmooth = Vector3.SmoothDamp(ragdollParent.transform.position, targetPosition, ref velocity, moveTime, moveMaxSpeed);
            Quaternion targetRotationSmooth = Quaternion.RotateTowards(ragdollParent.transform.rotation, targetRotation, rotateTime);
            // MOVE ragdollparent
            // rigidbodyToDrag.MovePosition(targetPositionSmooth);
            // rigidbodyToDrag.MoveRotation(targetRotationSmooth);
            ragdollParent.position = targetPositionSmooth;
            ragdollParent.rotation = targetRotationSmooth;
        }
    }

    public void OnDragEnd(Vector2 screenPos)
    {
        if (!draggingEnabled)
            return;
            
        if (debugLog)
            Debug.Log(gameObject.name + " OnDragEnd");

        rigidbodyToDrag.isKinematic = false;
    }
}
