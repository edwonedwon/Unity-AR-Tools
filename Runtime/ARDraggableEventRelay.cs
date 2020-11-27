using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ARDraggableEventRelay : MonoBehaviour
{
    public UnityEvent onDragBegin;
    public UnityEvent onDragUpdate;
    public UnityEvent onDragEnd;

    public void OnDragBegin(GameObject dragged, Vector2 screenPos)
    {
        onDragBegin.Invoke();
    }

    public void OnDragUpdate(GameObject dragged, Vector2 screenPos)
    {
        onDragUpdate.Invoke();
    }

    public void OnDragEnd(GameObject dragged, Vector2 screenPos)
    {
        onDragEnd.Invoke();
    }

    void OnEnable()
    {
        ARDraggableEventBroadcaster.onDragBeginEvent += OnDragBegin;
        ARDraggableEventBroadcaster.onDragUpdateEvent += OnDragUpdate;
        ARDraggableEventBroadcaster.onDragEndEvent += OnDragEnd;
    }

    void OnDisable()
    {
        ARDraggableEventBroadcaster.onDragBeginEvent -= OnDragBegin;
        ARDraggableEventBroadcaster.onDragUpdateEvent -= OnDragUpdate;
        ARDraggableEventBroadcaster.onDragEndEvent -= OnDragEnd;
    }
}
