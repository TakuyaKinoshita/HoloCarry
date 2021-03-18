using System;
using UnityEngine;
using UnityEngine.Events;

namespace HoloCarry
{
    public class MapObject : MonoBehaviour, StartInterface
    {
        [SerializeField] private UnityEvent OnStartGame;
        public void OnStartSystem()
        {
            this.OnStartGame?.Invoke();
        }
    }
}