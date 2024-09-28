using System;
using System.Collections.Generic;
using EnvironmentControllers;
using UnityEngine;

namespace DefaultNamespace.ScriptableObjects
{
    [CreateAssetMenu(fileName = "Team", menuName = "Team", order = 0)]
    public class TeamSO : ScriptableObject
    {
        public GameObject m_WormPrefab;
        public Color m_Color;
        public int m_PlayersQuantity;
    }
}