using System;
using EnvironmentControllers;
using TMPro;
using UnityEngine;

public class TurnTimer : MonoBehaviour
{
    public int m_TurnLength = 30;
    public int m_EscapingLength = 8;
        
    [SerializeField] private TextMeshProUGUI m_TimerLabel;
    [SerializeField] private EnvironmentController m_Environment;
        
    private float _timeAmount = 0f;
    public float m_TimeLeft
    {
        get => _timeAmount;
    }

    // Events
    public Action OnTimeOver;
        
    private void Start()
    {
        m_Environment.OnTurnStarted += env =>
            _timeAmount = (float)m_TurnLength;
            
        m_Environment.OnEscapePhaseStarted += env =>
            _timeAmount = (float)m_EscapingLength;
    }

    private void Update()
    {
        if (_timeAmount > 0f)
        {
            _timeAmount -= Time.deltaTime;
            if (_timeAmount <= 0f)
            {
                // _timeAmount = 0;
                OnTimeOver?.Invoke();
            }
                
            m_TimerLabel.text = ((int)_timeAmount).ToString();
        }
    }
}