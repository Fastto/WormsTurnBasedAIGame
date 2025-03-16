using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace WormComponents
{
    public class Weapon : MonoBehaviour
    {
        [SerializeField] private GameObject m_ProjectilePrefab;
        public Transform m_LaunchPoint;
        [SerializeField] private WormController m_WormController;
        [SerializeField] private GameObject m_AimSprite;
        [SerializeField] private float m_MinScale = 1f;
        [SerializeField] private float m_MaxScale = 3f;
        public bool m_PowerCanBeControlled = true;

        private float _power = 1f;
        public float m_Power
        {
            get => _power;
            set
            {
                _power = value < 0f ? 0f : value > 1f ? 1f : value;
                var scale = m_MinScale + (m_MaxScale - m_MinScale) * value;
                m_AimSprite.transform.localScale = new Vector3(scale, scale, scale);
            }
        }

        private void Start()
        {
            m_AimSprite.SetActive(false);
            m_WormController.m_State.OnChange +=state =>
            {
                m_AimSprite.SetActive(state == WormState.States.SHOOTING);
            };
        }

        public void Fire()
        {
            var projectileGo = Instantiate(m_ProjectilePrefab, m_LaunchPoint.position, Quaternion.identity);
            projectileGo.transform.right = transform.right;
            projectileGo.GetComponent<Projectile>().Launch(GetProjectileVelocity());
        }
        
        public List<Vector2> CalculateTrajectory(int maxSteps, float timeStep)
        {
            var startPosition = m_LaunchPoint.position;
            var initialVelocity = GetProjectileVelocity();
                
            var points = new List<Vector2>();
            var gravity = Physics2D.gravity;

            for (int i = 0; i < maxSteps; i++)
            {
                float time = i * timeStep;
                
                float x = startPosition.x + initialVelocity.x * time;
                float y = startPosition.y + initialVelocity.y * time + 0.5f * gravity.y * time * time;

                Vector2 newPosition = new Vector2(x, y);
                points.Add(newPosition);
            }

            return points;
        }
        
        public Vector2 GetProjectileVelocity()
        {
            return transform.right * (m_Power * 10 + 5);
        }
        
        public void RandomConfig()
        {
            transform.Rotate(new Vector3(0, 0, Random.Range(0, 360)));
            
            if(m_PowerCanBeControlled)
                m_Power = Random.value;
        }
    }
}
