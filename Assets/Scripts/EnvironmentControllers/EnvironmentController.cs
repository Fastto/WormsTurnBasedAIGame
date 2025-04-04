using System;
using System.Collections.Generic;
using Destructible2D;
using UnityEngine;
using Random = UnityEngine.Random;

namespace EnvironmentControllers
{
    public class EnvironmentController : MonoBehaviour
    {
        public TurnTimer m_Timer;
        [SerializeField] protected  List<SpawningPoint> m_SpawningPoints;

        // Events
        public Action<EnvironmentController> OnGameStarted;
        public Action<EnvironmentController> OnTurnStarted;
        public Action<EnvironmentController> OnEscapePhaseStarted;
        public Action<EnvironmentController> OnTurnFinished;

        public D2dDestructibleSprite m_Map;

        public virtual void Reset()
        {
        
        }
        
        public virtual void RefreshMap()
        {
            if (m_Map != null)
            {
                m_Map.Rebuild();
            }
        }
    
        public void PlaceWormRandomly(List<WormController> worms)
        {
            var points = new List<SpawningPoint>(m_SpawningPoints);
            foreach (var worm in worms)
            {
                var point = points[Random.Range(0, points.Count)];
                points.Remove(point);
                worm.transform.position = point.transform.position;
            }
        }

        public WormController InstantiateWorm(GameObject prefab)
        {
            var worm = Instantiate(prefab, transform).GetComponent<WormController>();
            worm.m_EnvironmentController = this;
            return worm;
        }
    }
}
