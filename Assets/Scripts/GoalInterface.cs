using UnityEngine;

namespace HoloCarry
{
    public interface GoalTrrigerInterface
    {
        void OnGoal(Collider collider);
        void OffGoal(Collider collider);
    }
}