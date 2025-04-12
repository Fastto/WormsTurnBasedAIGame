using Unity.MLAgents;
using Util;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WormComponents.Agents.Easy
{
    public class ComputerPlayerEasy : WormAgentEasy
    {
        private new void Start()
        {
            _ignoreEpisodeBegin = true;
            StartCoroutine(AssignEnemyWhenReady());
            
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

        private IEnumerator AssignEnemyWhenReady()
        {
            yield return new WaitForSeconds(0.1f);
            SetEnemy(RandomEnemy.GetRandomEnemy(m_WormController.GetColor()));
        }
    }
}
