using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Rigidbody2D _rigidbody;
    public int m_Damage = 34;

    public float m_LifeTime = 5f;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    public virtual int GetDamage()
    {
        return m_Damage;
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        Destroy(gameObject);
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
        yield return new WaitForSeconds(m_LifeTime); //todo remove hardcode
        Destroy(gameObject);
    }
}
