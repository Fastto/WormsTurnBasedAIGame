using System.Collections.Generic;
using EnvironmentControllers;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;
using UnityEngine.Serialization;

namespace WormComponents
{
    public class WormAgent : Agent
    {
        [SerializeField] protected WormController m_WormController;
        public int m_RefreshMapAfterEpisodes = 50;
        
        protected int _episodesCount;

        public override void OnActionReceived(ActionBuffers actions) {

            // Actions processing 
            var xAxis = actions.DiscreteActions[0] - 1; 
            var yAxis = actions.DiscreteActions[1] - 1; 
            var jumpButton = actions.DiscreteActions[2]; 
            var fireButton = actions.DiscreteActions[3];

            switch (m_WormController.m_State.m_State)
            {
                case WormState.States.MOVING:
                    m_WormController.m_ControllingSignals.m_HorizontalMoving = xAxis;
                    m_WormController.m_ControllingSignals.m_Jump = jumpButton == 1;
                    m_WormController.m_ControllingSignals.m_Aim = fireButton == 1;
                    break;
                case WormState.States.SHOOTING:
                    m_WormController.m_ControllingSignals.m_Aimning = xAxis;
                    m_WormController.m_ControllingSignals.m_PowerChanging = yAxis;
                    // m_WormController.m_ControllingSignals.m_AimCancel = jumpButton == 1;
                    m_WormController.m_ControllingSignals.m_Fire = jumpButton == 1;
                    break;
                case WormState.States.ESCAPING:
                    m_WormController.m_ControllingSignals.m_HorizontalMoving = xAxis;
                    m_WormController.m_ControllingSignals.m_Jump = jumpButton == 1;
                    break;
                default:
                    break;
            }
        }

        // todo: stacking input
        public override void Heuristic(in ActionBuffers actionsOut) {
            ActionSegment<int> discreteActions = actionsOut.DiscreteActions;
            discreteActions[0] = Input.GetAxis("Horizontal") > 0f ? 2 : Input.GetAxis("Horizontal") < 0f ? 0 : 1;
            discreteActions[1] = Input.GetAxis("Vertical") > 0f ? 2 : Input.GetAxis("Vertical") < 0f ? 0 : 1;
            discreteActions[2] = Input.GetButton("Jump") ? 1 : 0;
            discreteActions[3] = Input.GetButton("Fire1") ? 1 : 0;
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
            }
        }
        
        public virtual void ApplyEnemyLayers(LayerMask lm)
        {
        }
    }
}
