using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace WormComponents
{
    // Controlling Signals
    public class ControllingSignals : MonoBehaviour
    {
        [SerializeField]  private WormController m_WormController;
        
        // HORIZONTAL MOVING
        protected int _horizontalMoving = 0;
        public int m_HorizontalMoving
        {
            get => _horizontalMoving;
            set
            {
                if (value != 0 && _horizontalMoving == 0) OnMovingStarted?.Invoke(m_WormController.m_RigidBody);
                else if(value == 0 && _horizontalMoving != 0) OnMovingStopped?.Invoke(m_WormController.m_RigidBody);
                _horizontalMoving = value;
            }
        }

        public bool m_Jump = false;

        protected bool _aim = false;
        public bool m_Aim
        {
            get => _aim;
            set
            {
                if (_aim) OnMovingStopped?.Invoke(m_WormController.m_RigidBody);
                _aim = value;
            }
        }
        
        public bool m_AimCancel = false;
        public bool m_Fire = false;
        public int m_Aimning = 0;
        public int m_PowerChanging = 0;
        
        public int m_WeaponId = 0;
        public int m_TargetWeaponId = 0;
        
        public float m_SetPower = 0;
        public Vector2 m_SetAim = Vector2.zero;
        
        // Events
        public Action<Rigidbody2D> OnMovingStarted;
        public Action<Rigidbody2D> OnMovingStopped;
    }
}