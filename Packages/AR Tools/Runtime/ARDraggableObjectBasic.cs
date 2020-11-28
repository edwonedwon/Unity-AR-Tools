using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IARDraggableObject
{
    void OnDragBegin(Vector2 screenPos);
    void OnDragUpdate(Vector2 screenPos);
    void OnDragEnd(Vector2 screenPos);
}

public class ARDraggableObjectBasic : MonoBehaviour, IARDraggableObject
{
    public Vector2 screenPos {get;set;}
    GameObject player;
    Camera playerCamera;
    public LayerMask raycastLayerMask;
    public bool debugDraw;

    void Awake()
    {
        player = GameObject.FindWithTag("Player");
        playerCamera = player.GetComponentInChildren<Camera>();
    }

    public void OnDragBegin(Vector2 screenPos)
    {
        transform.LookAt(playerCamera.transform);
    }

    public void OnDragUpdate(Vector2 screenPos)
    {
        // update rotation
        Ray ray = playerCamera.ScreenPointToRay(screenPos);
        if (debugDraw)
            Debug.DrawRay(ray.origin, ray.direction, Color.red, .001f);
        if (Physics.Raycast(ray, out RaycastHit hit, 10000000, raycastLayerMask))
        {
            if (debugDraw)
                Debug.DrawRay(hit.point, Vector3.up, Color.green, .001f);
            transform.position = hit.point;
        }

        // update position
        transform.up = Vector3.up;
        transform.forward = Vector3.ProjectOnPlane(playerCamera.transform.position - hit.point, Vector3.up);
    }

    public void OnDragEnd(Vector2 screenPos)
    {

    }
}
