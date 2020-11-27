using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ARDraggableEventBroadcaster : MonoBehaviour, IARDraggableObject
{
    public delegate void OnDragBeginEvent(GameObject dragged, Vector2 screenPos);
    public delegate void OnDragUpdateEvent(GameObject dragged, Vector2 screenPos);
    public delegate void OnDragEndEvent(GameObject dragged, Vector2 screenPos);
    public static OnDragBeginEvent onDragBeginEvent;
    public static OnDragUpdateEvent onDragUpdateEvent;
    public static OnDragEndEvent onDragEndEvent;

    public void OnDragBegin(Vector2 screenPos)
    {
        if (onDragBeginEvent != null)
            onDragBeginEvent(gameObject, screenPos);
    }

    public void OnDragUpdate(Vector2 screenPos)
    {
        if (onDragUpdateEvent != null)
            onDragUpdateEvent(gameObject, screenPos);
    }

    public void OnDragEnd(Vector2 screenPos)
    {
        if (onDragEndEvent != null)
            onDragEndEvent(gameObject, screenPos);
    }
}
