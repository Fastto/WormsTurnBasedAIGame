using System.Collections.Generic;
using DefaultNamespace.ScriptableObjects;
using EnvironmentControllers;
using UnityEngine;

public class Team
{
    protected List<WormController> m_Worms = new List<WormController>();
    protected EnvironmentController _environment;
    protected TeamSO _settings;
        
    public Team(EnvironmentController env, TeamSO settings)
    {
        _environment = env;
        _settings = settings;
    }
        
    public List<WormController> GetWorms()
    {
        return m_Worms;
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
            worm.SetColor(_settings.m_Color);
            m_Worms.Add(worm);
            worm.OnDie += w =>
            {
                if (m_Worms.Contains(w)) m_Worms.Remove(w);
            };
        }
    }
}