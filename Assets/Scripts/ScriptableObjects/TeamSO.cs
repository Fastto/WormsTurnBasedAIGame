using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "Team", menuName = "Team", order = 0)]
    public class TeamSO : ScriptableObject
    {
        public GameObject m_WormPrefab;
        public Color m_UI_Color;
        public Color m_Worm_Color;
        public int m_PlayersQuantity;

        public int m_WormLayerId;
        public LayerMask m_EnemyLayers;
    }
}