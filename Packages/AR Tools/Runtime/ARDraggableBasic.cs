using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Edwon.MobileTools;

namespace Edwon.ARTools
{
    public interface IARDraggable
    {
        void OnDragBegin(Vector2 screenPos);
        void OnDragUpdate(Vector2 screenPos);
        void OnDragEnd(Vector2 screenPos);
    }

    public interface IARDraggableSpawnable
    {
        Vector3 GetSpawnPosition(Vector2 screenPos);
    }

    public class ARDraggableBasic : MonoBehaviour, IARDraggable, IARDraggableSpawnable
    {
        public Vector2 screenPos {get;set;}
        GameObject player;
        Camera playerCamera;
        public enum PlacementType { Touch, Raycast }
        public PlacementType placementType;
        [Header("Touch")]
        public float distanceFromCamera = 1f; // only relevent if set to Touch placement type
        [Header("Raycast")]
        public LayerMask placementRaycastLayerMask; // only relevent if set to Raycast placement type
        [Header("Debug")]
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

            if (placementType == PlacementType.Touch)
            {
                Vector3 position = playerCamera.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, distanceFromCamera));
                transform.position = position;
            }
            else if (placementType == PlacementType.Raycast)
            {
                if (Physics.Raycast(ray, out RaycastHit hit, 10000000, placementRaycastLayerMask))
                {
                    if (debugDraw)
                        Debug.DrawRay(hit.point, Vector3.up, Color.green, .001f);
                    transform.position = hit.point;
                }
                // update position
                transform.up = Vector3.up;
                transform.forward = Vector3.ProjectOnPlane(playerCamera.transform.position - hit.point, Vector3.up);
            }
        }

        public void OnDragEnd(Vector2 screenPos)
        {

        }

        public Vector3 GetSpawnPosition(Vector2 screenPos)
        {
            return Vector3.zero;
        }
    }
}