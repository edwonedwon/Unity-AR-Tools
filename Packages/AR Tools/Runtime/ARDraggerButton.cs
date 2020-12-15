using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;
using Stately;
using System;

namespace Edwon.ARTools
{
    [RequireComponent(typeof(Button))]
    public class ARDraggerButton : MonoBehaviour,
        IPointerDownHandler,
        IDragHandler,
        IPointerUpHandler,
        IScrollHandler,
        IPointerExitHandler
    {
        public UnityEvent onPointerHold;
        public UnityEvent onVerticalDragEnter;
        public UnityEvent onVerticalDragExit;

        public GameObject draggablePrefab;
        IARDraggable[] draggables;

        public float pointerHoldTime = 1f;

        Vector2 startPos;
        Vector2 screenPos;
        Button button;
        ScrollRect scrollRect;

        PointerEventData currentEventData;

        private string stateDebugString;
        private string stateDebugStringLast;
        public bool debugUI = false;
        public bool debugLogState = false;

        State rootState = new State("root");
        State idleState = new State("idle");
        State pointerHold = new State("pointerHold");
        State pointerDown = new State("pointerDown");
        State pointerUp = new State("pointerUp");
        PointerDraggingState draggingState = new PointerDraggingState("dragging");
        public class PointerDraggingState : State 
        {
            public PointerDraggingState(string name) : base(name) { }
            public State init = new State("init");
            public State draggingVerticaly = new State("draggingVerticaly");
            public State scrollingHorizontaly = new State("scrollingHorizontaly");
        }

        const string onPointerDownSignal = "onPointerDownSignal";
        const string onPointerUpSignal = "onPointerUp";
        const string onPointerExitSignal = "onPointerExit";
        const string onDragSignal = "onDragSignal";

        GameObject player;
        Camera playerCamera;

        void Awake()
        {
            player = GameObject.FindWithTag("Player");
            playerCamera = player.GetComponentInChildren<Camera>();

            button = GetComponent<Button>();
            scrollRect = GetComponentInParent<ScrollRect>();

            DefineStateMachine();
            rootState.Start();
        }

        void Update()
        {
            rootState.Update(Time.deltaTime);
            UpdateStateDebugString();
        }
        
        void UpdateStateDebugString()
        {
            stateDebugString = rootState.CurrentStatePath;
            if (stateDebugString != stateDebugStringLast)
            {
                if (debugLogState)
                    Debug.Log("DraggablePuppetChooseButton: STATE " + stateDebugString);
            }
            stateDebugStringLast = stateDebugString;
        }

        void DefineStateMachine()
        {
            rootState.StartAt(idleState);
            idleState.ChangeTo(pointerDown).IfSignalCaught(onPointerDownSignal);
            pointerDown.ChangeTo(draggingState).IfSignalCaught(onDragSignal);
            pointerDown.ChangeTo(pointerHold).After(pointerHoldTime);
            pointerDown.ChangeTo(idleState).IfSignalCaught(onPointerUpSignal).OrIfSignalCaught(onPointerExitSignal);
            pointerHold.ChangeTo(idleState).IfSignalCaught(onPointerUpSignal).OrIfSignalCaught(onPointerExitSignal);
            draggingState.ChangeTo(pointerUp).IfSignalCaught(onPointerUpSignal);
            pointerUp.ChangeTo(pointerDown).IfSignalCaught(onPointerDownSignal);

            pointerDown.OnEnter = delegate
            {
                OnPointerDown(currentEventData);
            };

            pointerHold.OnEnter = delegate
            {
                onPointerHold.Invoke();
            };

            draggingState.StartAt(draggingState.init);
            {
                draggingState.OnUpdate = delegate
                {
                    OnDrag(currentEventData);
                };
                draggingState.init.ChangeTo(draggingState.draggingVerticaly).If
                (
                    ()=> (Mathf.Abs(screenPos.x - startPos.x) < Mathf.Abs(screenPos.y - startPos.y))
                );
                draggingState.init.ChangeTo(draggingState.scrollingHorizontaly).If
                (
                    ()=> (Mathf.Abs(screenPos.x - startPos.x) > Mathf.Abs(screenPos.y - startPos.y))
                );
        
                draggingState.draggingVerticaly.OnEnter = delegate
                {
                    if (onVerticalDragEnter != null)
                        onVerticalDragEnter.Invoke();
                    button.onClick.Invoke();
                    button.OnPointerExit(currentEventData);

                    // get spawn position
                    IARDraggableSpawnable spawnable = draggablePrefab.GetComponent<IARDraggableSpawnable>();
                    Vector3 spawnPosition = Vector3.zero;
                    if (spawnable != null)
                        spawnPosition = spawnable.GetSpawnPosition(screenPos);

                    // spawn
                    GameObject draggableInstance = GameObject.Instantiate(draggablePrefab, spawnPosition, Quaternion.identity);

                    // send drag begin event
                    draggables = draggableInstance.GetComponents<IARDraggable>();
                    foreach(IARDraggable draggable in draggables)
                        draggable.OnDragBegin(screenPos);
                };
                draggingState.draggingVerticaly.OnUpdate = delegate
                {
                    foreach(IARDraggable draggable in draggables)
                        draggable.OnDragUpdate(screenPos);
                };
                draggingState.draggingVerticaly.OnFixedUpdate = delegate
                {
                    
                };
                draggingState.draggingVerticaly.OnExit = delegate
                {
                    if (onVerticalDragExit != null)
                        onVerticalDragExit.Invoke();
                    foreach(IARDraggable draggable in draggables)
                        draggable.OnDragEnd(screenPos);
                };

                draggingState.scrollingHorizontaly.OnEnter = delegate
                {
                    if (scrollRect != null)
                        scrollRect.OnBeginDrag(currentEventData);
                };
                draggingState.scrollingHorizontaly.OnUpdate = delegate
                {
                    if (scrollRect != null)
                        scrollRect.OnDrag(currentEventData);
                };
                draggingState.scrollingHorizontaly.OnExit = delegate
                {
                    if (scrollRect != null)
                        scrollRect.OnEndDrag(currentEventData);
                };

                pointerUp.OnEnter = delegate
                {
                    OnPointerUp(currentEventData);
                };
            }
        }

        public void OnPointerDown(PointerEventData eventData) 
        {
            #if UNITY_EDITOR
            startPos = Input.mousePosition;
            #else
            startPos = Input.GetTouch(0).position;
            #endif

            currentEventData = eventData;
            rootState.SendSignal(onPointerDownSignal);
        }

        public void OnDrag(PointerEventData eventData) 
        {
            #if UNITY_EDITOR
            screenPos = Input.mousePosition;
            #else
            screenPos = Input.GetTouch(0).position;
            #endif

            currentEventData = eventData;

            rootState.SendSignal(onDragSignal);
        }

        public void OnPointerUp(PointerEventData eventData) 
        {
            currentEventData = eventData;
            rootState.SendSignal(onPointerUpSignal);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            currentEventData = eventData;
            rootState.SendSignal(onPointerExitSignal);
        }

        public void OnScroll(PointerEventData eventData) 
        {
            if (scrollRect != null)
                scrollRect.OnScroll(eventData);
        }

        public void SetDraggablePrefab(GameObject prefab)
        {
            draggablePrefab = prefab;
        }
    }
}