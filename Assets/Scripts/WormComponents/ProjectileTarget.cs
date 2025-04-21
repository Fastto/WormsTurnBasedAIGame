using System;
using UnityEngine;

namespace WormComponents
{
    public class ProjectileTarget : MonoBehaviour
    {
        public Action<int> OnTouch;
    
        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.TryGetComponent(out Projectile projectile)) {
                OnTouch?.Invoke(projectile.GetDamage());
                Destroy(projectile.gameObject);
            }
        }
    }
}
