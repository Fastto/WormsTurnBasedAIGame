using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace EnvironmentControllers
{
    public class EnvironmentController : MonoBehaviour
    {
        // not UI, but in scene game timer.
        // it is necessary to make it possible to have several environments with personal timers
        public TurnTimer m_Timer;
        
        // points to spawn worms
        [SerializeField] protected  List<SpawningPoint> m_SpawningPoints;

        // Events
        public Action<EnvironmentController> OnGameStarted;
        public Action<EnvironmentController> OnTurnStarted;
        public Action<EnvironmentController> OnEscapePhaseStarted;
        public Action<EnvironmentController> OnTurnFinished;

        public virtual void Reset()
        {
        
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
