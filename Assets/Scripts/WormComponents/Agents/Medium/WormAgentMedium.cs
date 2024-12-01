using System.Collections.Generic;
using ScriptableObjects;
using Unity.MLAgents.Sensors;
using UnityEngine;

namespace WormComponents.Agents.Medium
{
    public class WormAgentMedium : WormAgent
    {
        protected bool _ignoreEpisodeBegin = false;
        [SerializeField] private List<WormController> m_Puppets;
        [SerializeField] private TeamSO m_MyTeamSettings;
        [SerializeField] private TeamSO m_EnemyTeamSettings;

        protected void Start()
        {
            m_WormController.SetColor(m_MyTeamSettings.m_Color);
            m_WormController.ApplyLayer(m_MyTeamSettings.m_WormLayerId);
            ApplyEnemyLayers(m_MyTeamSettings.m_EnemyLayers);
            m_WormController.m_IsSelfDestroyAllowed = false;

            // We have 3 ,appets on scene
            // 1st is friend, rest are enemies
            // sp that we pay fine for the targeting first and rewarding for the rest
            m_Puppets[0].m_IsSelfDestroyAllowed = false;
            m_Puppets[0].SetColor(m_MyTeamSettings.m_Color);
            m_Puppets[0].ApplyLayer(m_MyTeamSettings.m_WormLayerId);
            m_Puppets[0].m_ProjectileTarget.OnTouch += damage =>
            {
                SetReward(-1f); // penalty for fist
                EndEpisode();
            };

            for (int i = 1; i < m_Puppets.Count; i++)
            {
                m_Puppets[i].m_IsSelfDestroyAllowed = false;
                m_Puppets[i].SetColor(m_EnemyTeamSettings.m_Color);
                m_Puppets[i].ApplyLayer(m_EnemyTeamSettings.m_WormLayerId);
                m_Puppets[i].m_ProjectileTarget.OnTouch += damage =>
                {
                    SetReward(1f); //  rewards for rest
                    EndEpisode();
                };
            }

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
        }

        public override void OnEpisodeBegin()
        {
            if (_ignoreEpisodeBegin)
                return;

            m_WormController.Reset();
            m_WormController.m_Weapon.RandomConfig();

            foreach (var puppet in m_Puppets)
            {
                puppet.Reset();
            }

            var worms = new List<WormController>();
            worms.Add(m_WormController);
            m_Puppets.ForEach(p => worms.Add(p));
            m_WormController.m_EnvironmentController.PlaceWormRandomly(worms);
            m_WormController.m_EnvironmentController.Reset();
        }

        public override void CollectObservations(VectorSensor sensor)
        {
            sensor.AddObservation(m_WormController.m_EnvironmentController.m_Timer.m_TimeLeft
                                  / m_WormController.m_EnvironmentController.m_Timer
                                      .m_TurnLength); // time to end of the phase
            sensor.AddObservation((int)m_WormController.m_State.m_State); // current phase
            sensor.AddObservation(m_WormController.m_Weapon.transform.right); // weapon direction
            sensor.AddObservation(m_WormController.m_Weapon.m_Power); // weapon force
            sensor.AddObservation(m_WormController.m_GroundDetector.m_IsGrounded); // Is Grounded
            
            var points = m_WormController.m_Weapon.CalculateTrajectory(20, 0.1f);
            foreach (var point in points)
            {
                sensor.AddObservation(((Vector2)m_WormController.transform.position - point).magnitude);
                sensor.AddObservation(((Vector2)m_WormController.transform.position - point).normalized);
                Debug.DrawLine(m_WormController.m_Weapon.m_LaunchPoint.position, point);
            }
        }
    }
}
