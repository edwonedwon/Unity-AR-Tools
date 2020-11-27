using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ARDraggerScreenTouch : ScreenTouchEventReceiverBase
{
    IARDraggableObject[] draggables;

    void Awake()
    {
        draggables = GetComponentsInChildren<IARDraggableObject>();
    }

    public override void OnTouchBegin(Vector2 screenPos)
    {
        foreach(IARDraggableObject draggable in draggables)
            draggable.OnDragBegin(screenPos);

    }

    public override void OnTouchUpdate(Vector2 screenPos)
    {
        foreach (IARDraggableObject draggable in draggables)
            draggable.OnDragUpdate(screenPos);
    }

    public override void OnTouchEnd(Vector2 screenPos)
    {
        foreach (IARDraggableObject draggable in draggables)
            draggable.OnDragEnd(screenPos);
    }
}
