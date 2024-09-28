using System;
using Unity.MLAgents;

namespace WormComponents.Agents
{
    public class WormAgentPlaying : WormAgent
    {
        private void Start()
        {
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
