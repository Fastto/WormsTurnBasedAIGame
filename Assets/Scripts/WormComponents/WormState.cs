using System;
using UnityEngine;

namespace WormComponents
{
    public class WormState : MonoBehaviour
    {
        public Action<States> OnChange;
        
        public enum States
        {
            // Idle - State of waiting its turn
            // Idle => Moving. Transition happens on Turn starting
            IDLE = 0, 
            
            // Moving
            // Moving => Shooting,
            // Moving => Idle (time over)
            MOVING = 1, 
            
            // Shooting
            // Shooting => Moving (Shooting Cancelation)
            // Shooting => Escaping (Shot done)
            // Shooting => Idle (time over)
            SHOOTING = 2,
            
            // Escaping
            // Escaping => Idle (time over)
            ESCAPING = 3
        }

        public States _state = States.IDLE;

        public States m_State
        {
            get => _state;
            set
            {
                _state = value;
                OnChange?.Invoke(value);
            }
        }
    }
}