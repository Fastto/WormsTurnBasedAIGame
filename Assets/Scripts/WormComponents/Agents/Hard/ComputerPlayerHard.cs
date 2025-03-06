using Unity.MLAgents;
using WormComponents.Agents.Medium;

namespace WormComponents.Agents.Hard
{
    public class ComputerPlayerHard : WormAgentHard
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
