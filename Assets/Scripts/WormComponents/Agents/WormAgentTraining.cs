using System.Collections.Generic;

namespace WormComponents.Agents
{
    public class WormAgentTraining : WormAgent
    {
        protected  void Start(){
            // rewarding if enemy has been damaged
            m_EnemyWormController.m_ProjectileTarget.OnTouch += damage => {
                SetReward(1f);
                EndEpisode();
            };

            m_WormController.m_EnvironmentController.OnTurnStarted += env =>
            {
                m_WormController.m_State.m_State = WormState.States.MOVING;
            };

            m_WormController.m_EnvironmentController.m_Timer.OnTimeOver += () =>
            {
                if (m_WormController.m_State.m_State != WormState.States.ESCAPING)
                {
                    SetReward(-1);
                }
                EndEpisode();
            };

            m_WormController.m_IsSelfDestroyAllowed = false;
            m_EnemyWormController.m_IsSelfDestroyAllowed = false;
        }

        public override void OnEpisodeBegin()
        {
            m_WormController.Reset();
            m_WormController.m_Weapon.RandomConfig();
            
            m_EnemyWormController.Reset();
            m_WormController.m_EnvironmentController.PlaceWormRandomly(
                new List<WormController>{m_WormController, m_EnemyWormController});

            m_WormController.m_EnvironmentController.Reset();
        }
    }
}
