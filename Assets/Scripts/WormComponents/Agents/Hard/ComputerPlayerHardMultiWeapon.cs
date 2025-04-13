using Unity.MLAgents;
using Util;
using WormComponents.Agents.Medium;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WormComponents.Agents.Hard
{
    public class ComputerPlayerHardMultiWeapon : WormAgentHardMultiWeapon
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
                    SetPlayEnemies(RandomEnemy.GetAllFriends(m_WormController), RandomEnemy.GetAllEnemies(m_WormController));
                    GetComponent<DecisionRequester>().enabled = true;
                }
            };
        }

        private IEnumerator AssignEnemyWhenReady()
        {
            yield return new WaitForSeconds(0.1f);
            SetPlayEnemies(RandomEnemy.GetAllFriends(m_WormController), RandomEnemy.GetAllEnemies(m_WormController));
        }
    }
}
