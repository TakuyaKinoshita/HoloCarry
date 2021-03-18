using System;
using System.Threading;
using UniRx.InternalUtil;
using UniRx.Operators;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AI;
using NaughtyAttributes;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Input;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UniRx;

namespace HoloCarry
{
    public class UnityChanManager : MonoBehaviour, IMixedRealityPointerHandler, GoalTrrigerInterface, StartInterface
    {
        #region Defined Enum
        public enum RotateMode
        {
            TargetForward = 1,
            TargetBackForward = 2,
            TargetLeft = 3,
            TargetRight = 4,
            PlayerForward = 10,
            PlayerBackForward = 11,
            Custom = 99,
        }
        public enum UnityChanState
        {
            UnInitialize,
            Initialize,
            Move,
            Goal,
            Start,
            Rotate,
            Pinch
        }
        #endregion

        #region Serialize Private Fields
        [SerializeField, BoxGroup("Nav Mesh 設定")] private NavMeshAgent agent;
        [SerializeField, BoxGroup("Nav Mesh 設定")] private GameObject[] navMeshes;
        [SerializeField, BoxGroup("Nav Mesh 設定")] private GameObject pipe;
        [SerializeField, BoxGroup("Nav Mesh 設定")] private Transform goal;
        [SerializeField, BoxGroup("UnityChan Setting")] private GameObject own;
        [SerializeField, BoxGroup("UnityChan Setting")] private Rigidbody rigidBody;
        [SerializeField, BoxGroup("UnityChan 回転設定")] private UnityEvent Rotated;
        [SerializeField, NaughtyAttributes.ReadOnly, BoxGroup("UnityChan GrabEvent")] private Vector3 grabStartPosition;
        [SerializeField, BoxGroup("UnityChan GrabEvent")] private UnityEvent pointerDown;
        [SerializeField, BoxGroup("UnityChan GrabEvent")] private UnityEvent pointerClicked;
        [SerializeField, BoxGroup("UnityChan GrabEvent")] private UnityEvent pointerDragged;
        [SerializeField, BoxGroup("UnityChan GrabEvent")] private UnityEvent pointerUp;
        [SerializeField, BoxGroup("UnityChan Animation Settings")] private Animator animator;
        [SerializeField, NaughtyAttributes.ReadOnly, BoxGroup("UnityChan Animation Settings")] private string pinchStr = "Pinch";
        [SerializeField, NaughtyAttributes.ReadOnly, BoxGroup("UnityChan Animation Settings")] private string speedStr = "Speed";
        [SerializeField, NaughtyAttributes.ReadOnly, BoxGroup("UnityChan Animation Settings")] private string GoalStr = "Goal";
        [SerializeField, BoxGroup("System Goal Event")] private UnityEvent onGoalGameEvent;
        [SerializeField, BoxGroup("System Goal Event")] private UnityEvent offGoalGameEvent;
        [SerializeField, BoxGroup("System Start Event")] private UnityEvent onStartGameEvent;
        #endregion

        #region Serialize Private Fields with Public Propeties
        [SerializeField, NaughtyAttributes.ReadOnly, BoxGroup("UnityChan Settings")] private UnityChanState state = UnityChanState.UnInitialize;
        public UnityChanState State
        {
            get => this.state;
            private set => this.state = value;
        }
        #endregion;

        #region Private Fields
        private IMixedRealityHandJointService handJointService;
        private Transform RightIndexTipTransform;
        private MixedRealityPose pose;
        #endregion

        #region MonoBehaviour Methods
        private void Awake()
        {
            this.own = this.transform.gameObject;
            this.state = UnityChanState.UnInitialize;
        }
        private void Start()
        {
            this.agent.enabled = false;
        }
        private void Update()
        {
            if (this.State == UnityChanState.Start)
            {
                agent.destination = goal.transform.position;
                this.State = UnityChanState.Move;
            }
            else if (this.State == UnityChanState.Move)
            {
                if (this.agent.isOnNavMesh)
                {
                    agent.destination = goal.transform.position;
                    var speed = agent.velocity.magnitude / transform.localScale.y;
                    animator.SetFloat("Speed", speed);
                }
            } else if (this.State == UnityChanState.Pinch) {
                if (HandJointUtils.TryGetJointPose(TrackedHandJoint.IndexTip, Handedness.Right, out pose))
                {
                    Observable.Timer(TimeSpan.FromSeconds(0.2f))
                    .Subscribe(_ => {
                        // this.agent.Warp(this.pose.Position);
                        this.own.transform.position = this.pose.Position;
                        this.own.transform.rotation = pose.Rotation;
                    });
                }
            }
        }
        #endregion

        #region Nav Mesh Event Methods
        public void StartNavMesh()
        {
            StartCoroutine(RunMesh());
        }
        #endregion

        #region Grab Event Methods
        public void OnPointerDown(MixedRealityPointerEventData eventData)
        {
            if (eventData.Pointer is SpherePointer)
            {
                if (this.State == UnityChanState.Rotate) return;
                if (this.State == UnityChanState.Goal) this.animator.SetBool(GoalStr, false);
                Debug.Log($"Grab Pointer Down start from {eventData.Pointer.PointerName}");
                this.grabStartPosition = this.own.transform.position;
                this.agent.enabled = false;
                this.State = UnityChanState.Pinch;
                this.animator.SetBool(pinchStr, true);
                pointerDown?.Invoke();
            }
        }
        public void OnPointerClicked(MixedRealityPointerEventData eventData)
        {
            if (eventData.Pointer is SpherePointer)
            {
                if (this.State == UnityChanState.Rotate) return;
                Debug.Log($"Grab Pointer Clicked start from {eventData.Pointer.PointerName}");
                pointerClicked?.Invoke();
            }
        }
        public void OnPointerDragged(MixedRealityPointerEventData eventData)
        {
            if (eventData.Pointer is SpherePointer)
            {
                if (this.State == UnityChanState.Rotate) return;
                Debug.Log($"Grab Pointer Dragged start from {eventData.Pointer.PointerName}");
                pointerDragged?.Invoke();
            }
        }
        public void OnPointerUp(MixedRealityPointerEventData eventData)
        {
            if (eventData.Pointer is SpherePointer)
            {
                if (this.State == UnityChanState.Rotate) return;
                Debug.Log($"Grab Pointer Up start from {eventData.Pointer.PointerName}");
                this.own.transform.position = this.grabStartPosition;
                this.own.transform.LookAt(this.goal, Vector3.up);
                this.agent.enabled = true;
                this.agent.Warp(this.grabStartPosition);
                this.animator.SetBool(pinchStr, false);
                this.State = UnityChanState.Move;
                pointerUp?.Invoke();
            }
        }
        #endregion

        #region Goal Trriger Interface Methods
        public async void OnGoal(Collider other)
        {
            if (this.State != UnityChanState.Rotate && this.State != UnityChanState.Pinch)
            {
                this.State = UnityChanState.Rotate;
                Quaternion targetRotation = GetLookRotation(RotateMode.PlayerForward);
                await RotateOverTime(this.transform, targetRotation, 1.5f, this.Rotated);
            }
            if (this.State != UnityChanState.Pinch)
            {
                this.State = UnityChanState.Goal;
                animator.SetBool(GoalStr, true);
            }
        }
        public void OffGoal(Collider other)
        {
            if (this.State != UnityChanState.Pinch) this.State = UnityChanState.Move;
            animator.SetBool(GoalStr, false);
        }
        #endregion

        #region Start Trriger Interface Methods
        public void OnStartSystem()
        {
            this.State = UnityChanState.Initialize;
            this.StartNavMesh();
        }
        #endregion

        #region Utility Methods
        public void RotatePlayerForward(float _duration = 1.5f)
        {
            if (this.State != UnityChanState.Rotate)
            {
                this.State = UnityChanState.Rotate;
                Quaternion targetRotation = GetLookRotation(RotateMode.PlayerForward);
                StartCoroutine(RotateOverTime(this.transform, targetRotation, _duration, this.Rotated));
            }
            else
            {
                Debug.LogWarning("ERROR : Can`t Start RotateMode ** RotatePlayerForward.");
            }
        }
        /// <summary>
        /// モードがTargetForward又はTargetBackForwardの場合のみ第二引数のGameobject _targetに引数を与えてください。
        /// </summary>
        /// <param name="_mode"></param>
        /// <param name="_target"></param>
        /// <returns></returns>
        private Quaternion GetLookRotation(RotateMode _mode, Transform _direction = null)
        {
            // vector3 cache
            Vector3 rotateEuler = Vector3.zero;
            // return quternion
            Quaternion lookRotation = new Quaternion();
            if (_mode == RotateMode.PlayerForward || _mode == RotateMode.PlayerBackForward)
            {
                _direction = Camera.main.transform;
            }

            Vector3 direction = _direction.position - transform.position;
            direction.y = 0;
            // Vector3.right   :  X軸回転
            // Vector3.up      :  y軸回転
            // Vector3.forward :  z軸回転
            lookRotation = Quaternion.LookRotation(direction, Vector3.up);

            if (_mode == RotateMode.PlayerBackForward || _mode == RotateMode.TargetBackForward)
            {
                // 逆方向の回転を取得する
                lookRotation = Quaternion.Inverse(lookRotation);
            }
            return lookRotation;
        }
        #endregion

        #region Coroutine methods
        public IEnumerator RunMesh()
        {
            // mavmeshareaの有効化
            if (navMeshes.Length > 0) {
                foreach (var t in navMeshes)
                {
                    if ( t != null) t.AddComponent<NavMeshSourceTag>();
                }
            }
            yield return null;
            agent.enabled = true;
            yield return null;
            Debug.Log("agent Status: " + agent.isOnNavMesh);
            if (pipe != null) pipe.SetActive(false);
            yield return new WaitForSeconds(1.0f);
            this.State = UnityChanState.Start;
            yield break;
        }
        private IEnumerator RotateOverTime(Transform transformToRotate, Quaternion targetRotation, float duration, UnityEvent _callback = null)
        {
            var startRotation = transformToRotate.rotation;

            var timePassed = 0f;
            while (timePassed < duration)
            {
                var factor = timePassed / duration;
                // optional add ease-in and -out
                //factor = Mathf.SmoothStep(0, 1, factor);

                animator.SetFloat(speedStr, 0.8f);
                transformToRotate.rotation = Quaternion.Lerp(startRotation, targetRotation, factor);
                // or
                //transformToRotate.rotation = Quaternion.Slerp(startRotation, targetRotation, factor);

                // increae by the time passed since last frame
                timePassed += Time.deltaTime;

                // important! This tells Unity to interrupt here, render this frame
                // and continue from here in the next frame
                yield return null;
            }

            // to be sure to end with exact values set the target rotation fix when done
            transformToRotate.rotation = targetRotation;
            _callback?.Invoke();
            this.State = UnityChanState.Goal;
        }
        #endregion
    }
}
