using System;
using Unity.VisualScripting;
using UnityEngine;

namespace WormComponents
{
    public class GroundDetector : MonoBehaviour
    {
        public bool m_IsGrounded { get; private set; }

        private void OnCollisionStay2D(Collision2D other)
        {
            m_IsGrounded = true;
        }

        private void OnCollisionExit2D(Collision2D other)
        {
            m_IsGrounded = false;
        }
    }
}