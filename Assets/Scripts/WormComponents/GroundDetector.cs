using System;
using CW.Common;
using UnityEngine;
using Unity.VisualScripting;

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

        private void OnCollisionStay2D(Destructible2D.D2dEdgeCollider other)
        {
            m_IsGrounded = true;
        }

        private void OnCollisionExit2D(Destructible2D.D2dEdgeCollider other)
        {
            m_IsGrounded = false;
        }
    }
}