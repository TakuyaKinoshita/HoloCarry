using UnityEngine;
using UnityEngine.Events;

namespace HoloCarry
{
    public class CarryBridgeObject : MonoBehaviour, StartInterface
    {
        [SerializeField] private UnityEvent OnStartEvent;
        public void OnStartSystem()
        {
            this.OnStartEvent?.Invoke();
        }
    }
}