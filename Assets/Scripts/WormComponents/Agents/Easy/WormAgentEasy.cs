using System.Collections.Generic;
using Unity.MLAgents.Sensors;
using UnityEngine;

namespace WormComponents.Agents.Easy
{
    public class WormAgentEasy : WormAgent
    {
        protected bool _ignoreEpisodeBegin = false;
        [SerializeField] protected WormController m_EnemyWormController;
        
        public void SetEnemy(WormController worm)
        {
            m_EnemyWormController = worm;
        }
        
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

            m_WormController.m_Health.OnDie += () =>
            {
                SetReward(-1);
                EndEpisode();
            };

            m_WormController.m_IsSelfDestroyAllowed = false;
            m_EnemyWormController.m_IsSelfDestroyAllowed = false;
        }

        public override void OnEpisodeBegin()
        {
            if(_ignoreEpisodeBegin)
                return;
            
            _episodesCount++;
            if (_episodesCount >= m_RefreshMapAfterEpisodes)
            {
                _episodesCount -= m_RefreshMapAfterEpisodes;
                m_WormController.m_EnvironmentController.RefreshMap();
            }
            
            m_WormController.Reset();
            m_WormController.m_ActiveWeapon.RandomConfig();
            
            m_EnemyWormController.Reset();
            m_WormController.m_EnvironmentController.PlaceWormRandomly(
                new List<WormController>{m_WormController, m_EnemyWormController});

            m_WormController.m_EnvironmentController.Reset();
        }
        
        public override void CollectObservations(VectorSensor sensor)
        {
            if (m_EnemyWormController != null)
            {
                var direction = (m_EnemyWormController.transform.position - m_WormController.transform.position)
                    .normalized;
                var distance = (m_EnemyWormController.transform.position - m_WormController.transform.position)
                    .magnitude;

                sensor.AddObservation(direction); // vector to enemy
                sensor.AddObservation(distance); // distance to enemy
            }
            else
            {
                sensor.AddObservation(Vector3.zero); // vector to enemy
                sensor.AddObservation(0f); // distance to enemy
            }

            sensor.AddObservation(m_WormController.m_EnvironmentController.m_Timer.m_TimeLeft 
                                  / m_WormController.m_EnvironmentController.m_Timer.m_TurnLength); // time to end of the phase
            sensor.AddObservation((int)m_WormController.m_State.m_State); // current phase
            sensor.AddObservation(m_WormController.m_ActiveWeapon.transform.right); // weapon direction
            sensor.AddObservation(m_WormController.m_ActiveWeapon.m_Power); // weapon force
            sensor.AddObservation(m_WormController.m_GroundDetector.m_IsGrounded); // Is Grounded


            var points = m_WormController.m_ActiveWeapon.CalculateTrajectory(20, 0.1f);
            if (m_EnemyWormController != null)
            {
                foreach (var point in points)
                {
                    sensor.AddObservation(((Vector2)m_EnemyWormController.transform.position - point).magnitude);
                    Debug.DrawLine(m_WormController.m_ActiveWeapon.m_LaunchPoint.position, point);
                }
            }
            else
            {
                for (int i = 0; i <= 20; i++)
                {
                    sensor.AddObservation(0);
                }
            }

        }
    }
}
