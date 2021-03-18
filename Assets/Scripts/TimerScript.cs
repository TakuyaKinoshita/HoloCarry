using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;

namespace HoloCarry
{
    public class TimerScript : MonoBehaviour, StartInterface, GoalTrrigerInterface
    {

        [SerializeField]
        private int minute;
        [SerializeField]
        private float seconds;
        //　前のUpdateの時の秒数
        private float oldSeconds;
        //　タイマー表示用テキスト
        [SerializeField] private TextMeshProUGUI timerText;
        private bool isStopped = true;
        public bool IsStopped
        {
            get => isStopped;
            set => isStopped = value;
        }

        void Start()
        {
            minute = 0;
            seconds = 0f;
            oldSeconds = 0f;
            timerText = transform.GetComponent<TextMeshProUGUI>();
        }

        void Update()
        {
            if (!isStopped) {
                seconds += Time.deltaTime;
                if (seconds >= 60f)
                {
                    minute++;
                    seconds = seconds - 60;
                }
                //　値が変わった時だけテキストUIを更新
                if ((int)seconds != (int)oldSeconds)
                {
                    timerText.text = minute.ToString("00") + ":" + ((int)seconds).ToString("00");
                }
                oldSeconds = seconds;
            }
        }
        public void OnStartSystem()
        {
            this.IsStopped = false;
        }
        public void OnGoal(Collider other)
        {
            this.IsStopped = true;
        }
        public void OffGoal(Collider other)
        {
        }
    }
}