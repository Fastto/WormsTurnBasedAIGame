using UnityEngine;

namespace WormComponents
{
    public class FrictionController : MonoBehaviour
    {
        [SerializeField] private PhysicsMaterial2D m_NoFrictionMaterial;
        [SerializeField] private PhysicsMaterial2D m_FullFrictionMaterial;
        [SerializeField] private WormController m_WormController;

        private void Start()
        {
            m_WormController.m_ControllingSignals.OnMovingStarted += rigidBody =>
                rigidBody.sharedMaterial = m_NoFrictionMaterial;
            
            m_WormController.m_ControllingSignals.OnMovingStopped += rigidBody =>
                rigidBody.sharedMaterial = m_FullFrictionMaterial;

            m_WormController.m_State.OnChange += state =>
            {
                if (state == WormState.States.IDLE || state == WormState.States.SHOOTING)
                    m_WormController.m_RigidBody.sharedMaterial = m_FullFrictionMaterial;
            };
        }

      
    }
}