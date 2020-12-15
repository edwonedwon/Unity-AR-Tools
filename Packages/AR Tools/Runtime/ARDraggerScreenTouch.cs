using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Edwon.MobileTools;

namespace Edwon.ARTools
{
    public class ARDraggerScreenTouch : ScreenTouchEventReceiverBase
    {
        IARDraggable[] draggables;

        void Awake()
        {
            draggables = GetComponentsInChildren<IARDraggable>();
        }

        public override void OnTouchBegin(Vector2 screenPos)
        {
            foreach(IARDraggable draggable in draggables)
                draggable.OnDragBegin(screenPos);

        }

        public override void OnTouchUpdate(Vector2 screenPos)
        {
            foreach (IARDraggable draggable in draggables)
                draggable.OnDragUpdate(screenPos);
        }

        public override void OnTouchEnd(Vector2 screenPos)
        {
            foreach (IARDraggable draggable in draggables)
                draggable.OnDragEnd(screenPos);
        }
    }
}
