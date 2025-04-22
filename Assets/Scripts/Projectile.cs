using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Destructible2D;
using WormComponents;

public class Projectile : MonoBehaviour
{
    private Rigidbody2D _rigidbody;
    public int m_Damage = 34;

    public float m_LifeTime = 5f;
    
    private bool hitDestructible = false;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    public virtual int GetDamage()
    {
        return m_Damage;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<ProjectileTarget>() != null)
            return;

        if (collision.gameObject.GetComponent<D2dCollider>() != null)
        {
            hitDestructible = true;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (hitDestructible && other.gameObject.GetComponent<D2dCollider>() != null)
        {
            Destroy(gameObject);
        }
    }

    public void Launch(Vector2 velocity)
    {
        StartCoroutine(LaunchAfterFrame(velocity));
    }

    private IEnumerator LaunchAfterFrame(Vector2 velocity)
    {
        yield return null;
        _rigidbody.velocity = velocity;
        StartCoroutine(SelfDestroy());
    }
    
    private IEnumerator SelfDestroy()
    {
        yield return new WaitForSeconds(m_LifeTime);
        Destroy(gameObject);
    }
}
