using System;
using System.Collections.Generic;
using EnvironmentControllers;
using ScriptableObjects;
using UnityEngine;
using WormComponents;

public class Team
{
    protected List<WormController> m_Worms = new List<WormController>();
    protected EnvironmentController _environment;
    protected TeamSO _settings;

    protected int _currentWorm = -1;
        
    public Team(EnvironmentController env, TeamSO settings)
    {
        _environment = env;
        _settings = settings;
    }
        
    public List<WormController> GetWorms()
    {
        return m_Worms;
    }

    public WormController GetNextWorm()
    {
        if (m_Worms.Count == 0)
        {
            throw new Exception("No worms found");
        }
        
        if (_currentWorm == -1)
        {
            _currentWorm = 0;
        }
        else
        {
            _currentWorm += 1;
        }

        if (_currentWorm >= m_Worms.Count)
        {
            _currentWorm = 0;
        }

        return m_Worms[_currentWorm];
    }

    public void DestroyAllWorms()
    {
        while (m_Worms.Count > 0)
        {
            var worm = m_Worms[0];
            m_Worms.Remove(worm);
            worm.SelfDestroy();
        }
    }

    public void Reset()
    {
        for (int i = 0; i < _settings.m_PlayersQuantity; i++)
        {
            var worm = _environment.InstantiateWorm(_settings.m_WormPrefab);
            worm.SetColor(_settings.m_UI_Color);
            worm.ApplyLayer(_settings.m_WormLayerId);
            if (worm.TryGetComponent(out WormAgent agent))
            {
                agent.ApplyEnemyLayers(_settings.m_EnemyLayers);
            }
                
            m_Worms.Add(worm);
            worm.OnDie += w =>
            {
                if (m_Worms.Contains(w)) m_Worms.Remove(w);
            };
        }
    }
}