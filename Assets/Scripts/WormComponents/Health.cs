using System;
using UnityEngine;

namespace WormComponents
{
    public class Health : MonoBehaviour
    {
        public int m_StartingHealth = 100;
        private int _health;

        private void Start()
        {
            Reset();
        }

        public int m_Health
        {
            get => _health;
            set
            {
                _health = value > 0 ? value : 0;
                OnChange?.Invoke(_health);
                
                if (_health <= 0) 
                    OnDie?.Invoke();
            }
        }

        public void Reset()
        {
            m_Health = m_StartingHealth;
        }

        public Action<int> OnChange;
        public Action OnDie;
    }
}