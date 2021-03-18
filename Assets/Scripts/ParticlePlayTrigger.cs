using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace HoloCarry
{
    public class ParticlePlayTrigger : MonoBehaviour, GoalTrrigerInterface
    {
        [SerializeField] private ParticleSystem[] particles;
        public UnityEvent GoalCallBack;
        public UnityEvent OffGoalCallBack;
        void Start()
        {
            foreach (var particle in this.particles)
            {
                particle.Clear();
                particle.Pause();
            }
        }
        public void OnGoal(Collider other)
        {
            foreach (var particle in this.particles)
            {
                particle.Play();
            }
            this.GoalCallBack?.Invoke();
        }
        public void OffGoal(Collider other)
        {
            foreach (var particle in this.particles)
            {
                particle.Clear();
                particle.Pause();
            }
            this.OffGoalCallBack?.Invoke();
        }
    }
}

