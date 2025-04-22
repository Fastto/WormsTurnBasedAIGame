using System.Collections.Generic;
using Unity.MLAgents.Sensors;
using UnityEngine;
using Unity.MLAgents.Actuators;

namespace WormComponents.Agents.Easy
{
    public class WormAgentEasy : WormAgent
    {
        protected bool _ignoreEpisodeBegin = false;
        [SerializeField] protected WormController m_EnemyWormController;
        
        protected  void Start(){
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
        
        public void SetEnemy(WormController worm)
        {
            m_EnemyWormController = worm;
        }
        public override void OnActionReceived(ActionBuffers actions) {

            // Actions processing 
            var xAxis = actions.DiscreteActions[0] - 1; 
            var yAxis = actions.DiscreteActions[1] - 1; 
            var jumpButton = actions.DiscreteActions[2]; 
            var fireButton = actions.DiscreteActions[3];
            var newWeaponId = actions.DiscreteActions[4];

            switch (m_WormController.m_State.m_State)
            {
                case WormState.States.MOVING:
                    m_WormController.m_ControllingSignals.m_HorizontalMoving = xAxis;
                    m_WormController.m_ControllingSignals.m_Jump = jumpButton == 1;
                    m_WormController.m_ControllingSignals.m_Aim = fireButton == 1;
                    m_WormController.m_ControllingSignals.m_TargetWeaponId = newWeaponId;
                    break;
                case WormState.States.SHOOTING:
                    m_WormController.m_ControllingSignals.m_Aimning = xAxis;
                    m_WormController.m_ControllingSignals.m_PowerChanging = yAxis;
                    // m_WormController.m_ControllingSignals.m_AimCancel = jumpButton == 1;
                    m_WormController.m_ControllingSignals.m_Fire = jumpButton == 1;
                    m_WormController.m_ControllingSignals.m_TargetWeaponId = newWeaponId;
                    break;
                case WormState.States.ESCAPING:
                    m_WormController.m_ControllingSignals.m_HorizontalMoving = xAxis;
                    m_WormController.m_ControllingSignals.m_Jump = jumpButton == 1;
                    break;
                default:
                    break;
            }
        }

        protected int GetNewWeaponIdFromInput()
        {
            int newWeaponId = m_WormController.m_ControllingSignals.m_WeaponId;

            if (Input.GetKey(KeyCode.Alpha1)) newWeaponId = 0;
            if (Input.GetKey(KeyCode.Alpha2)) newWeaponId = 1;
            if (Input.GetKey(KeyCode.Alpha3)) newWeaponId = 2;

            return newWeaponId;
        }
        
        public override void Heuristic(in ActionBuffers actionsOut) {
            ActionSegment<int> discreteActions = actionsOut.DiscreteActions;
            discreteActions[0] = Input.GetAxis("Horizontal") > 0f ? 2 : Input.GetAxis("Horizontal") < 0f ? 0 : 1;
            discreteActions[1] = Input.GetAxis("Vertical") > 0f ? 2 : Input.GetAxis("Vertical") < 0f ? 0 : 1;
            discreteActions[2] = Input.GetButton("Jump") ? 1 : 0;
            discreteActions[3] = Input.GetButton("Fire1") ? 1 : 0;
            discreteActions[4] = GetNewWeaponIdFromInput();
        }
        
        public override void WriteDiscreteActionMask(IDiscreteActionMask actionMask)
        {
            if (m_WormController.m_State.m_State == WormState.States.IDLE)
            {
                // X axis
                actionMask.SetActionEnabled(0, 0, false); 
                actionMask.SetActionEnabled(0, 1, true);
                actionMask.SetActionEnabled(0, 2, false); 
                
                // Y axis
                actionMask.SetActionEnabled(1, 0, false); 
                actionMask.SetActionEnabled(1, 1, true);
                actionMask.SetActionEnabled(1, 2, false); 
            
                // Jump
                actionMask.SetActionEnabled(2, 0, true);
                actionMask.SetActionEnabled(2, 1, false);

                // Fire
                actionMask.SetActionEnabled(3, 0, true);
                actionMask.SetActionEnabled(3, 1, false);
                
                // WeaponID
                actionMask.SetActionEnabled(4, 0, m_WormController.m_ControllingSignals.m_WeaponId == 0); 
                actionMask.SetActionEnabled(4, 1, m_WormController.m_ControllingSignals.m_WeaponId == 1);
                actionMask.SetActionEnabled(4, 2, m_WormController.m_ControllingSignals.m_WeaponId == 2);
            }
            else if (m_WormController.m_State.m_State == WormState.States.MOVING)
            {
                // X axis. Moving left / right
                actionMask.SetActionEnabled(0, 0, true); 
                actionMask.SetActionEnabled(0, 1, true); 
                actionMask.SetActionEnabled(0, 2, true); 
                
                // Y axis. Not Used
                actionMask.SetActionEnabled(1, 0, false); 
                actionMask.SetActionEnabled(1, 1, true);
                actionMask.SetActionEnabled(1, 2, false); 
            
                // Jump
                actionMask.SetActionEnabled(2, 0, true); 
                actionMask.SetActionEnabled(2, 1, m_WormController.m_GroundDetector.m_IsGrounded);

                // Fire -- Switching to aim
                actionMask.SetActionEnabled(3, 0, true); 
                actionMask.SetActionEnabled(3, 1, true);
                
                // WeaponID
                actionMask.SetActionEnabled(4, 0, m_WormController.m_ControllingSignals.m_WeaponId == 0); 
                actionMask.SetActionEnabled(4, 1, m_WormController.m_ControllingSignals.m_WeaponId == 1);
                actionMask.SetActionEnabled(4, 2, m_WormController.m_ControllingSignals.m_WeaponId == 2);
            } 
            else if (m_WormController.m_State.m_State == WormState.States.SHOOTING)
            {
                // X axis. Aim rotate left / right
                actionMask.SetActionEnabled(0, 0, true); 
                actionMask.SetActionEnabled(0, 1, true); 
                actionMask.SetActionEnabled(0, 2, true); 
                
                // Y axis. Shot Power + -
                actionMask.SetActionEnabled(1, 0, true); 
                actionMask.SetActionEnabled(1, 1, true);
                actionMask.SetActionEnabled(1, 2, true); 
            
                // Jump. Shot
                actionMask.SetActionEnabled(2, 0, true); 
                actionMask.SetActionEnabled(2, 1, true);

                // Fire. 
                actionMask.SetActionEnabled(3, 0, true); 
                actionMask.SetActionEnabled(3, 1, false);
                
                // Weapon id
                actionMask.SetActionEnabled(4, 0, true); 
                actionMask.SetActionEnabled(4, 1, true);
                actionMask.SetActionEnabled(4, 2, true);
            }
            else if (m_WormController.m_State.m_State == WormState.States.ESCAPING)
            {
                // X axis. Moving left / right
                actionMask.SetActionEnabled(0, 0, true); 
                actionMask.SetActionEnabled(0, 1, true); 
                actionMask.SetActionEnabled(0, 2, true); 
                
                // Y axis. Not Used
                actionMask.SetActionEnabled(1, 0, false); 
                actionMask.SetActionEnabled(1, 1, true);
                actionMask.SetActionEnabled(1, 2, false); 
            
                // Jump
                actionMask.SetActionEnabled(2, 0, true); 
                actionMask.SetActionEnabled(2, 1, m_WormController.m_GroundDetector.m_IsGrounded);

                // Fire. Not Used
                actionMask.SetActionEnabled(3, 0, true);
                actionMask.SetActionEnabled(3, 1, false);
                
                // Weapon id
                actionMask.SetActionEnabled(4, 0, m_WormController.m_ControllingSignals.m_WeaponId == 0); 
                actionMask.SetActionEnabled(4, 1, m_WormController.m_ControllingSignals.m_WeaponId == 1);
                actionMask.SetActionEnabled(4, 2, m_WormController.m_ControllingSignals.m_WeaponId == 2);
            }
        }
    }
}
