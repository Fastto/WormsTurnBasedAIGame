using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Util
{
    public static class RandomEnemy
    {
        public static WormController GetRandomEnemy(Color MyColor)
        {
            var worms = GameObject.FindObjectsByType<WormController>(FindObjectsSortMode.None);
            var enemyWorms = new List<WormController>();
            foreach (var worm in worms)
            {
                if (worm.m_Health.m_Health > 0 && worm.GetColor() != MyColor)
                {
                    enemyWorms.Add(worm);
                }
            }

            if (enemyWorms.Count == 0)
                throw new Exception("No Enemy Found");
            
            return enemyWorms[Random.Range(0, enemyWorms.Count)];
        }

        public static List<WormController> GetAllFriends(WormController wc)
        {
            var worms = GameObject.FindObjectsByType<WormController>(FindObjectsSortMode.None);
            var friendWorms = new List<WormController>();
            foreach (var worm in worms)
            {
                if (worm.m_Health.m_Health > 0 && worm != wc && worm.GetColor() == wc.GetColor())
                {
                    friendWorms.Add(worm);
                }
            }
            
            return friendWorms;
        }

        public static List<WormController> GetAllEnemies(WormController wc)
        {
            var worms = GameObject.FindObjectsByType<WormController>(FindObjectsSortMode.None);
            var enemyWorms = new List<WormController>();
            foreach (var worm in worms)
            {
                if (worm.m_Health.m_Health > 0 && worm != wc && worm.GetColor() != wc.GetColor())
                {
                    enemyWorms.Add(worm);
                }
            }
            
            return enemyWorms;
        }
    }
}