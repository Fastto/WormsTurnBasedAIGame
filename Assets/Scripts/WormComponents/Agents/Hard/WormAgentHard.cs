using System.Collections.Generic;
using ScriptableObjects;
using Unity.MLAgents.Sensors;
using UnityEngine;

namespace WormComponents.Agents.Hard
{
    public class WormAgentHard : WormAgent
    {
        protected bool _ignoreEpisodeBegin = false;
        [SerializeField] private List<WormController> m_friendly_Puppets;
        [SerializeField] private List<WormController> m_enemy_Puppets;
        [SerializeField] private TeamSO m_MyTeamSettings;
        [SerializeField] private TeamSO m_EnemyTeamSettings;
        [SerializeField] private Camera m_WormsCamera;
        
        protected void Start()
        {
            Debug.Log(m_WormController);
            Debug.Log(m_MyTeamSettings);
            m_WormController.SetColor(m_MyTeamSettings.m_UI_Color);
            m_WormController.ApplyLayer(m_MyTeamSettings.m_WormLayerId);
            ApplyEnemyLayers(m_MyTeamSettings.m_EnemyLayers);
            m_WormController.m_IsSelfDestroyAllowed = false;

            for (int i = 0; i < m_friendly_Puppets.Count; i++)
            {
                m_friendly_Puppets[i].m_IsSelfDestroyAllowed = false;
                m_friendly_Puppets[i].SetColor(m_MyTeamSettings.m_UI_Color);
                m_friendly_Puppets[i].ApplyLayer(m_MyTeamSettings.m_WormLayerId);
                m_friendly_Puppets[i].m_ProjectileTarget.OnTouch += damage =>
                {
                    SetReward(-1f); //  penalty for friends
                    EndEpisode();
                };
            }

            for (int i = 0; i < m_enemy_Puppets.Count; i++)
            {
                m_enemy_Puppets[i].m_IsSelfDestroyAllowed = false;
                m_enemy_Puppets[i].SetColor(m_EnemyTeamSettings.m_UI_Color);
                m_enemy_Puppets[i].ApplyLayer(m_EnemyTeamSettings.m_WormLayerId);
                m_enemy_Puppets[i].m_ProjectileTarget.OnTouch += damage =>
                {
                    SetReward(1f); //  rewards for enemies
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
            
            m_WormController.m_Health.OnDie += () =>
            {
                SetReward(-1);
                EndEpisode();
            };
        }

        public override void OnEpisodeBegin()
        {
            if (_ignoreEpisodeBegin)
                return;
            
            _episodesCount++;
            if (_episodesCount >= m_RefreshMapAfterEpisodes)
            {
                _episodesCount -= m_RefreshMapAfterEpisodes;
                m_WormController.m_EnvironmentController.RefreshMap();
            }

            m_WormController.Reset();
            m_WormController.m_ActiveWeapon.RandomConfig();

            foreach (var puppet in m_friendly_Puppets)
            {
                puppet.Reset();
            }

            foreach (var puppet in m_enemy_Puppets)
            {
                puppet.Reset();
            }

            var worms = new List<WormController>();
            worms.Add(m_WormController);
            m_friendly_Puppets.ForEach(p => worms.Add(p));
            m_enemy_Puppets.ForEach(p => worms.Add(p));
            m_WormController.m_EnvironmentController.PlaceWormRandomly(worms);
            m_WormController.m_EnvironmentController.Reset();
        }

        public override void CollectObservations(VectorSensor sensor)
        {
            sensor.AddObservation(m_WormController.m_EnvironmentController.m_Timer.m_TimeLeft
                                  / m_WormController.m_EnvironmentController.m_Timer
                                      .m_TurnLength); // time to end of the phase
            sensor.AddObservation((int)m_WormController.m_State.m_State); // current phase
            sensor.AddObservation(m_WormController.m_ActiveWeapon.transform.right); // weapon direction
            sensor.AddObservation(m_WormController.m_ActiveWeapon.m_Power); // weapon force
            sensor.AddObservation(m_WormController.m_GroundDetector.m_IsGrounded); // Is Grounded
            
            var points = m_WormController.m_ActiveWeapon.CalculateTrajectory(20, 0.1f);
            foreach (var point in points)
            {
                sensor.AddObservation(((Vector2)m_WormController.transform.position - point).magnitude);
                sensor.AddObservation(((Vector2)m_WormController.transform.position - point).normalized);
                Debug.DrawLine(m_WormController.m_ActiveWeapon.m_LaunchPoint.position, point);
            }
        }
        
        public override void ApplyEnemyLayers(LayerMask lm)
        {
            m_WormsCamera.cullingMask = lm;
        }
    }
}
