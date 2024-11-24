using Unity.MLAgents;
using Util;

namespace WormComponents.Agents.Easy
{
    public class ComputerPlayerEasy : WormAgentEasy
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
                    SetEnemy(RandomEnemy.GetRandomEnemy(m_WormController.GetColor()));
                    GetComponent<DecisionRequester>().enabled = true;
                }
            };
        }
    }
}
