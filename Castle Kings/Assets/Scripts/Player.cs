using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {
    enum Sides {
        BACK,
        RIGHT,
        FRONT,
        LEFT
    };

    // Private const
    private const float SPEED_DEFAULT = 0.05f;
    private const float SPEED_MINIMUM = 0.025f;

    // Components
    Rigidbody2D m_rbody2D;
    Animator m_animator;
    SpriteRenderer m_sprRen;
    ParticleSystem m_particles;

    // Sides
    Sides m_currentSide;
    Sides m_lastSide;

    // States
    bool m_isMoving;
    bool m_isAttacking;
    [HideInInspector] public bool m_carryPrincess;
    bool m_dead;

    // Variables
    float m_speed;
    private int m_score;
    [HideInInspector] public int m_ID;

    


	// Use this for initialization
	void Start () {
        m_ID = 0;
        m_animator = GetComponent<Animator>();
        m_rbody2D = GetComponent<Rigidbody2D>();
        m_sprRen = GetComponent<SpriteRenderer>();
        m_particles = GetComponent<ParticleSystem>();
        m_currentSide = Sides.FRONT;
        m_lastSide = Sides.FRONT;
        m_isMoving = false;
        m_isAttacking = false;
        m_carryPrincess = false;
        m_dead = false;
        m_speed = SPEED_DEFAULT;
        m_particles.Stop();
        Spawn();
	}
	
	// Update is called once per frame
	void Update () {
        if (m_currentSide != m_lastSide)
        {
            m_lastSide = m_currentSide;
            if (!m_isAttacking)
            {
                SetAnimation();
            }
            //Debug.Log("Updated!");
        }

        if (m_isAttacking)
        {
            CheckForAttackEnd();
        }

        if (m_dead)
        {
    //      Debug.Log(m_animator.GetCurrentAnimatorStateInfo(0).IsName("Death"));
    //      if (!m_animator.GetCurrentAnimatorStateInfo(0).IsName("Death"))
    //      {
    //          Debug.Log("GROO2");
    //          m_dead = false;
    //         // m_sprRen.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
    //      }
        }
	}

    private void CheckForAttackEnd() {
        switch (m_currentSide)
        {
            case Sides.BACK:
                if (!m_animator.GetCurrentAnimatorStateInfo(0).IsName("BackAttack"))
                {
                    AttackEnded();
                }
                break;

            case Sides.RIGHT:
                if (!m_animator.GetCurrentAnimatorStateInfo(0).IsName("RightAttack"))
                {
                    AttackEnded();
                }
                break;

            case Sides.FRONT:
                if (!m_animator.GetCurrentAnimatorStateInfo(0).IsName("FrontAttack"))
                {
                    AttackEnded();
                }
                break;

            case Sides.LEFT:
                if (!m_animator.GetCurrentAnimatorStateInfo(0).IsName("LeftAttack"))
                {
                    AttackEnded();
                }
                break;
        } 
    }

    private void AttackEnded() {
        m_isAttacking = false;
        m_isMoving = false;
        m_animator.speed = 0;
    }

    // Init the corrent walking animation or idle (frame 0) base on the currentSide
    private void SetAnimation() {
        switch (m_currentSide)
        {
            case Sides.BACK:
                m_animator.Play("BackWalk", -1, 0);
                break;

            case Sides.RIGHT:
                m_animator.Play("RightWalk", -1, 0);
                break;

            case Sides.FRONT:
                m_animator.Play("FrontWalk", -1, 0);
                break;

            case Sides.LEFT:
                m_animator.Play("LeftWalk", -1, 0);
                break;
        }
    }

    // Init the correct attack animation base on the currentSide
    private void SetAttack() {
        switch (m_currentSide)
        {
            case Sides.BACK:
                m_animator.Play("BackAttack", -1, 0);
                break;

            case Sides.RIGHT:
                m_animator.Play("RightAttack", -1, 0);
                break;

            case Sides.FRONT:
                m_animator.Play("FrontAttack", -1, 0);
                break;

            case Sides.LEFT:
                m_animator.Play("LeftAttack", -1, 0);
                break;
        }
    }

    public void SetSide(float p_angle) {
        if (!m_isAttacking)
        {
            if (p_angle > -45 && p_angle <= 45)
            {
                m_currentSide = Sides.BACK;
            }
            else if (p_angle > 45 && p_angle <= 135)
            {
                m_currentSide = Sides.RIGHT;
            }
            else if (p_angle > 135 || p_angle <= -135)
            {
                m_currentSide = Sides.FRONT;
            }
            else
            {
                m_currentSide = Sides.LEFT;
            }
        }
    }

    public void Move(float p_X, float p_Y) {
        if (!m_isMoving)
        {
            m_animator.speed = 1;
            m_isMoving = true;
        }

        Vector3 moveDirection = new Vector3(p_X, p_Y, 0);
        moveDirection *= m_speed;
        m_rbody2D.transform.Translate(moveDirection);
    }

    public void StopMoving() {
        SetAnimation(); // Reset animation to get back to the first animation frame (idle)
        m_animator.speed = 0;
        m_isMoving = false;
    }

    public void Attack() {
        if (!m_isAttacking)
        {
            m_isAttacking = true;
            SetAttack();
            m_animator.speed = 1; // In case the player was in idle
        }
    }

    private void Spawn() {

    }

    // Event functions (call with SendMessage)
    void Killed()
    {
        if (!m_dead)
        {
            m_animator.Play("Death", -1, 0);
            if (m_carryPrincess)
                m_particles.Stop();
            m_dead = true;
        }
       
    }

    void TakePrincess()
    {
        m_carryPrincess = true;
        m_speed = SPEED_MINIMUM;
        Debug.Log("I shall bring you to safety my dear!");
        m_particles.Play();
    }

    void Score()
    {
        m_score += 150;
        m_particles.Stop();
        m_carryPrincess = false;
        m_speed = SPEED_DEFAULT;

        // TODO: Respawn princess

        Debug.Log(m_score);
    }

    public void IncreaseScore(int p_byAmount)
    {
        m_score += p_byAmount;
        Debug.Log(m_score);
    }

    public bool IsDead()
    {
        return m_dead;
    }
}
