using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Edwon.MobileTools;
using Edwon.Tools;

namespace Edwon.ARTools
{
    public class ARDraggerScreenTouch : ScreenTouchEventReceiverBase
    {
        IDraggable[] draggables;

        void Awake()
        {
            draggables = GetComponentsInChildren<IDraggable>();
        }

        public override void OnTouchBegin(Vector2 screenPos)
        {
            foreach(IDraggable draggable in draggables)
                draggable.OnDragBegin(screenPos);

        }

        public override void OnTouchUpdate(Vector2 screenPos)
        {
            foreach (IDraggable draggable in draggables)
                draggable.OnDragUpdate(screenPos);
        }

        public override void OnTouchEnd(Vector2 screenPos)
        {
            foreach (IDraggable draggable in draggables)
                draggable.OnDragEnd(screenPos);
        }
    }
}
