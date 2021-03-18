using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AI;
using NaughtyAttributes;

namespace KTToolkit.NavMesh
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class NavMeshManager : MonoBehaviour
    {
        #region Definition of Enum
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
        #endregion
        #region Private Serialize Fields
        /// <summary>
        /// NavMeshに関連しない標準的な内容を決定する
        /// </summary>
        [SerializeField, BoxGroup("Default Settings"), Tooltip("if you use this component not attach object.must set to use gameobject.")]
        private GameObject target = null;
        public GameObject Target
        {
            get => target;
            private set => target = value;
        }
        [SerializeField, BoxGroup("Default Settings")]
        private GameObject GoalObject;
        /// <summary>
        /// NavMeshに関連する設定
        /// </summary>
        [SerializeField, BoxGroup("NavMesh Settings")]
        private NavMeshAgent agent = null;
        public NavMeshAgent Agent
        {
            get => agent;
            private set => agent = value;
        }
        /// <summary>
        /// アニメーション関連のにかかわる設定
        /// </summary>
        [SerializeField, BoxGroup("Animation Settings")]
        private bool useAnimator = false;
        public bool UseAnimator
        {
            get => useAnimator;
            set => useAnimator = value;
        }
        [SerializeField, BoxGroup("Animation Settings"), ShowIf("ShowAnimation")]
        private Animator animator = null;
        [SerializeField, ShowIf("ShowAnimation"), ReadOnly, BoxGroup("Animation Settings")]
        private string attribute = "Speed";
        /// <summary>
        /// ターゲットの回転に関連するフィールド
        /// </summary>
        private Coroutine rotateCor = null;
        [SerializeField, BoxGroup("Callback Settings")]
        private UnityEvent GoalEvent;
        #endregion
        #region MonoBehaviour Functions
        private void Start()
        {
            this.rotateCor = null;
        }
        private void Update()
        {
            if (this.useAnimator)
            {
                var speed = agent.velocity.magnitude / transform.localScale.y;
                this.animator.SetFloat(attribute, speed, 0.1f, Time.deltaTime);
            }
        }
        private void OnEnable() { }
        private void OnDisable()
        {
            this.StopAllCoroutines();
        }
        #endregion
        #region Public Functions
        /// <summary>
        /// キャラクターを指定した場所まで移動させる。コールバックで終了時のアクションを実行できる
        /// <example>
        /// <code>
        /// MoveToPosition(Vector3.Zero, () => {  <br/>
        ///     // mode = nextmode;               <br/>
        ///     Debug.Log("到達")                  <br/>
        /// });
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="_position">目的地</param>
        /// <param name="_callback">到達時のコールバック</param>
        /// <returns></returns>
        public IEnumerator MoveToPosition(Vector3 _position, UnityEvent _callback)
        {
            bool moveEndFlg = true;
            _position.y = Camera.main.transform.position.y - 0.5f;
            this.agent.ResetPath();
            yield return new WaitForFixedUpdate();
            this.agent.destination = _position;
            yield return new WaitForFixedUpdate();
            while (moveEndFlg)
            {
                if (this.agent.pathPending)
                {
                    yield return null;
                }
                Debug.Log(this.agent.pathStatus);

                switch (this.agent.pathStatus)
                {
                    case NavMeshPathStatus.PathComplete:
                        // Debug.Log(this.agent.remainingDistance);
                        if (this.agent.remainingDistance < 0.1f)
                        {
                            Debug.Log("MoveToPosition is successfull. MoveTargetposition = " + _position);
                            moveEndFlg = false;
                        }
                        break;
                    case NavMeshPathStatus.PathPartial:
                    case NavMeshPathStatus.PathInvalid:
                        // 移動できない場所だった場合はワープしてくる
                        bool warp = this.agent.Warp(_position);
                        if (warp == false)
                        {
                            // warpできない場所だった際の処理
                            this.agent.enabled = false;
                            this.target.transform.position = _position;
                            this.agent.enabled = true;
                        }
                        moveEndFlg = false;
                        Debug.Log("MoveToPosition is success with Agent Warp Method. MoveTargetposition = " + _position);
                        break;
                    default:
                        Debug.Log("ERROR : 検知しないPathStatusが検出されました。 PathStatus => " + this.agent.pathStatus);
                        moveEndFlg = false;
                        break;
                }
                yield return null;
            }
            _callback?.Invoke();
        }
        /// <summary>
        /// Agentの機能を停止させる
        /// </summary>
        public void DisableAgent()
        {
            this.agent.enabled = false;
        }
        /// <summary>
        /// Agentの機能を開始する
        /// </summary>
        public void EnableAgent()
        {
            this.agent.enabled = true;
        }
        public void RotatePlayerForward(System.Action _callback = null, float _duration = 1.5f)
        {
            if (this.rotateCor == null)
            {
                Quaternion targetRotation = GetLookRotation(RotateMode.PlayerForward);
                this.rotateCor = StartCoroutine(RotateOverTime(this.target.transform, targetRotation, _duration, _callback));
            }
            else
            {
                Debug.LogWarning("ERROR : Can`t Start RotateMode ** RotatePlayerForward. Because rotateCor != null");
            }
        }
        public void RotatePlayerBackForward(System.Action _callback = null, float _duration = 1.5f)
        {
            if (this.rotateCor == null)
            {
                Quaternion targetRotation = GetLookRotation(RotateMode.PlayerBackForward);
                this.rotateCor = StartCoroutine(RotateOverTime(this.target.transform, targetRotation, _duration, _callback));
            }
            else
            {
                Debug.LogWarning("ERROR : Can`t Start RotateMode ** RotatePlayerBackForward. Because rotateCor != null");
            }
        }
        public void RotateTargetForward(Transform _target, System.Action _callback = null, float _duration = 1.5f)
        {
            if (this.rotateCor == null)
            {
                Quaternion targetRotation = GetLookRotation(RotateMode.TargetForward, _target);
                this.rotateCor = StartCoroutine(RotateOverTime(this.target.transform, targetRotation, _duration, _callback));
            }
            else
            {
                Debug.LogWarning("ERROR : Can`t Start RotateMode ** RotateTargetForward. Because rotateCor != null");
            }
        }
        public void RotateTargetBackForward(Transform _target, System.Action _callback = null, float _duration = 1.5f)
        {
            if (this.rotateCor == null)
            {
                Quaternion targetRotation = GetLookRotation(RotateMode.TargetBackForward, _target);
                this.rotateCor = StartCoroutine(RotateOverTime(this.target.transform, targetRotation, _duration, _callback));
            }
            else
            {
                Debug.LogWarning("ERROR : Can`t Start RotateMode ** RotateTargetBackForward. Because rotateCor != null");
            }
        }
        #endregion
        #region Private Functions
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

            Vector3 direction = _direction.position - this.target.transform.position;
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
        private IEnumerator RotateOverTime(Transform transformToRotate, Quaternion targetRotation, float duration, System.Action _callback = null)
        {
            var startRotation = transformToRotate.rotation;

            var timePassed = 0f;
            while (timePassed < duration)
            {
                var factor = timePassed / duration;
                // optional add ease-in and -out
                //factor = Mathf.SmoothStep(0, 1, factor);

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
            this.rotateCor = null;
        }
        #endregion
        #region NaughtyAttributes Functions
        private bool ShowAnimation()
        {
            return this.useAnimator;
        }
        #endregion

        #region Utility Event
        public void StartGame()
        {
            StartCoroutine(this.MoveToPosition(GoalObject.transform.position, this.GoalEvent));
        }
        #endregion
    }
}
