using System.Collections.Generic;
using UnityEngine;

namespace HoloCarry
{
    public class GoalFlagObject: MonoBehaviour
    {
        #region Serialize Private Fields
        [SerializeField] private GameObject[] TrrigerTarget;
        [SerializeField] private GameObject colliderTarget;
        #endregion

        #region Private Fields
        private List<GoalTrrigerInterface> goalInterfaces = new List<GoalTrrigerInterface>();
        #endregion

        #region MonoBehaviour methods
        private void Start()
        {
            foreach (var obj in this.TrrigerTarget)
            {
                if (obj.GetComponent<GoalTrrigerInterface>() != null)
                {
                    this.goalInterfaces.Add(obj.GetComponent<GoalTrrigerInterface>());
                }
            }
        }
        #endregion

        #region Collision Trriger methods
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject == colliderTarget)
            {
                foreach (var item in goalInterfaces)
                {
                    item.OnGoal(other);
                }
            }
        }
        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject == colliderTarget)
            {
                foreach (var item in goalInterfaces)
                {
                    item.OffGoal(other);
                }
            }
        }
        #endregion
    }
}