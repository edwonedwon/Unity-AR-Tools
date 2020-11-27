using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ARDraggableThrowable : MonoBehaviour, IARDraggableObject
{
    public Vector2 screenPos {get;set;}
    GameObject player;
    Camera playerCamera;
    Transform throwVectorTF;
    public Rigidbody mainRigidbody;
    public float distanceFromCamera = 1f;
    Vector2 screenVelocity;
    Vector2 screenVelocitySmoothed;
    Vector2 screenPosLast;
    public float throwForce = 0.1f;
    public float throwForceMin = 3f;
    public float throwForceClamp = 30;
    public float screenVelocityLerp = 0.9f;
    public bool randomRotation = true;
    public bool debugDraw;

    void Awake()
    {
        player = GameObject.FindWithTag("Player");
        playerCamera = player.GetComponentInChildren<Camera>();
        throwVectorTF = playerCamera.transform.Find("Throw Vector");
        if (mainRigidbody == null)
            mainRigidbody = GetComponent<Rigidbody>();
    }

    public void OnDragBegin(Vector2 screenPos)
    {   
        mainRigidbody.isKinematic = true;
        screenPosLast = Vector2.zero;
        if (randomRotation)
            mainRigidbody.MoveRotation(Random.rotation);
        else
            mainRigidbody.MoveRotation(playerCamera.transform.rotation);
    }

    public void OnDragUpdate(Vector2 screenPos)
    {
        Vector3 position = playerCamera.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, distanceFromCamera));
        
        mainRigidbody.MovePosition(position);

        screenVelocity = screenPos - screenPosLast;
        screenVelocitySmoothed = Vector3.Lerp(
            screenVelocitySmoothed, 
            new Vector3(screenVelocity.x, screenVelocity.y, screenVelocity.magnitude),
            screenVelocityLerp);
        screenPosLast = screenPos;
    }

    public void OnDragEnd(Vector2 screenPos)
    {
        mainRigidbody.isKinematic = false;
        Vector3 throwForceVector = new Vector3(
            Mathf.Clamp(screenVelocitySmoothed.x, -throwForceClamp, throwForceClamp), 
            0, 
            screenVelocitySmoothed.y);
        throwForceVector = throwVectorTF.TransformDirection(throwForceVector);
        throwForceVector *= throwForce;
        throwForceVector = Vector3.ClampMagnitude(throwForceVector, throwForceClamp);
        if (debugDraw)
            Debug.DrawRay(playerCamera.transform.position, throwForceVector, Color.red, 1f);
        mainRigidbody.AddForce(throwForceVector);
    }
}
