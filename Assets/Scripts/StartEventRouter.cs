using System.Collections.Generic;
using UnityEngine;

namespace HoloCarry
{
    public class StartEventRouter : MonoBehaviour
    {
        #region Prvate Serialize Field
        [SerializeField] private GameObject[] TrrigerEventObject;
        #endregion
        #region Private field
        private List<StartInterface> startInterface = new List<StartInterface>();
        #endregion
        private void Awake()
        {
            foreach (var obj in this.TrrigerEventObject)
            {
                if (obj.GetComponent<StartInterface>() != null)
                {
                    this.startInterface.Add(obj.GetComponent<StartInterface>());
                }
            }
        }
        public void StartEvent()
        {
            foreach (var item in this.startInterface)
            {
                item.OnStartSystem();
            }
        }
    }
}