using Unity.MLAgents;
using Util;
using WormComponents.Agents.Easy;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WormComponents.Agents.Medium
{
    public class ComputerPlayerMedium : WormAgentMedium
    {
        private new void Start()
        {
            _ignoreEpisodeBegin = true;
            
            m_WormController.m_State.OnChange += state =>
            {
                if (state == WormState.States.IDLE)
                {
                    GetComponent<DecisionRequester>().enabled = false;
                }
                else if (state == WormState.States.MOVING)
                {
                    GetComponent<DecisionRequester>().enabled = true;
                }
            };
        }
    }
}
