using System;
using EnvironmentControllers;
using UnityEngine;
using WormComponents;
using Random = UnityEngine.Random;

public class WormController : MonoBehaviour
{
    // Worm Characteristics
    [SerializeField] protected float m_Speed = 2;
    [SerializeField] protected float m_JumpSpeed = 4;
        
    // Worm Components
    public EnvironmentController m_EnvironmentController;
    
    public GroundDetector m_GroundDetector;
    public Weapon m_Weapon;
    public WormState m_State;
    public ControllingSignals m_ControllingSignals;
    public Rigidbody2D m_RigidBody;
    public Health m_Health;
    public HealthBar m_HealthBar;
    public ProjectileTarget m_ProjectileTarget;
    public SpriteRenderer m_SpriteRenderer;

    // Events
    public Action<WormController> OnReset;
    public Action<WormController> OnDie;

    public bool m_IsSelfDestroyAllowed = true;

    private void Start()
    {
        m_ProjectileTarget.OnTouch += 
            damage => m_Health.m_Health -= damage;

        m_Health.OnDie += () =>
        {
            OnDie?.Invoke(this);
            SelfDestroy();
        };

        m_EnvironmentController.m_Timer.OnTimeOver += () =>
        {
            if (m_State.m_State != WormState.States.IDLE)
            {
                m_State.m_State = WormState.States.IDLE;
            }
        };
    }

    public virtual void Reset()
    {
        m_RigidBody.velocity = Vector2.zero;
        m_RigidBody.angularVelocity = 0f;
        m_State.m_State = WormState.States.IDLE;
        m_Health.Reset();
        OnReset?.Invoke(this);
    }

    protected void Update()
    {
        //todo extract to component that checks is warm still on the map
        if (transform.localPosition.y < -15f)
        {
            m_Health.m_Health = -1; //DIE
            return;
        }
        
        if (m_State.m_State == WormState.States.MOVING && m_ControllingSignals.m_Aim)
        {
            m_State.m_State = WormState.States.SHOOTING;
            m_ControllingSignals.m_Aim = false;
        } 
        else if (m_State.m_State is WormState.States.MOVING or WormState.States.ESCAPING)
        {
            m_RigidBody.velocity = new Vector2(
                m_Speed * m_ControllingSignals.m_HorizontalMoving, 
                m_RigidBody.velocity.y);

            if (m_GroundDetector.m_IsGrounded && m_ControllingSignals.m_Jump)
            {
                m_RigidBody.velocity = new Vector2(m_RigidBody.velocity.x, m_JumpSpeed);
                m_ControllingSignals.m_Jump = false;
            }
        }
        else if (m_State.m_State == WormState.States.SHOOTING)
        {
            m_Weapon.transform.Rotate(new Vector3(0, 0, -m_ControllingSignals.m_Aimning * Time.deltaTime * 45f));
            m_Weapon.m_Power += m_ControllingSignals.m_PowerChanging * Time.deltaTime * 1f;
                
            if (m_ControllingSignals.m_AimCancel)
            {
                m_State.m_State = WormState.States.MOVING;
                m_ControllingSignals.m_AimCancel = false;
                return;
            }
                
            if (m_ControllingSignals.m_Fire)
            {
                m_Weapon.Fire();
                m_ControllingSignals.m_Fire = false;
                    
                m_State.m_State = WormState.States.ESCAPING;
                m_EnvironmentController.OnEscapePhaseStarted?.Invoke(m_EnvironmentController);
            }
        }
    }

    public void SelfDestroy()
    {
        if (m_IsSelfDestroyAllowed)
        {
            m_State.m_State = WormState.States.IDLE;
            Destroy(this.gameObject);
        }
    }

    public void SetColor(Color color)
    {
        m_SpriteRenderer.color = color;
        m_HealthBar.m_Text.color = color;
    }
    
    public Color GetColor()
    {
        return m_SpriteRenderer.color;
    }
}