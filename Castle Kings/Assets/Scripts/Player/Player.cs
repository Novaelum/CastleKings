using UnityEngine;
using System.Collections;

public class Player : Character {
    // Private const
    // Speed
    private const float SPEED_MAXIMUM = 0.1f;
    private const float SPEED_DEFAULT = 0.05f;
    private const float SPEED_MINIMUM = 0.025f;
    public float SPEED_DASH = 4f;

    // Manacost
    private const int MANACOST_DASH = 50;

    // Components
    Rigidbody2D m_rbody2D;
    Animator m_animator;
    SpriteRenderer m_sprRen;
    ParticleSystem m_particles;

    // Sides
    Sides m_currentSide;
    Sides m_lastSide;

    // States
    bool m_isMoving; // Whether or not movements keys are active
    bool m_isAttacking;
    bool m_isDashing;
    bool m_carryPrincess;
    bool m_dead;
    int m_score;

    // Variables
    private float m_speed;
    private int m_mana;
    private int m_health;
    [HideInInspector] public int m_ID;

    // Getters
    public int GetHealth() { return m_health; }
    public bool IsCarryingPrincess() { return m_carryPrincess; }

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
        m_mana = 100;
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
                SetAnimations("BackWalk", "RightWalk", "FrontWalk", "LeftWalk");
            }
            //Debug.Log("Updated!");
        }
	}

    // Init the corrent walking animation or idle (frame 0) base on the currentSide
    private void SetAnimations(string p_forBack, string p_forRight, string p_forFront, string p_forLeft) {
        switch (m_currentSide)
        {
            case Sides.BACK:
                m_animator.Play(p_forBack, -1, 0);
                break;

            case Sides.RIGHT:
                m_animator.Play(p_forRight, -1, 0);
                break;

            case Sides.FRONT:
                m_animator.Play(p_forFront, -1, 0);
                break;

            case Sides.LEFT:
                m_animator.Play(p_forLeft, -1, 0);
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

    public void Move(float p_X, float p_Y, float p_speed = 0) {
        if (!m_isMoving)
        {
            m_animator.speed = 1;
            m_isMoving = true;
        }

        Vector2 moveDirection = new Vector2(p_X, p_Y);
        if (p_speed == 0) {
            moveDirection *= m_speed;
            m_rbody2D.transform.Translate(moveDirection);
        }
        else
        {
            Debug.Log("TO");
            moveDirection *= p_speed;
            m_rbody2D.AddForce(moveDirection, ForceMode2D.Impulse);
            StartCoroutine(WaitForAndDo(1f, "DashEnded"));
        }
    }

    public void StopMoving() {
        SetAnimations("BackWalk", "RightWalk", "FrontWalk", "LeftWalk"); // Reset animation to get back to the first animation frame (idle)
        m_animator.speed = 0;
        m_isMoving = false;
    }

    public void Attack() {
        if (!m_isAttacking)
        {
            m_isAttacking = true;
            SetAnimations("BackAttack", "RightAttack", "FrontAttack", "LeftAttack");
            m_animator.speed = 1; // In case the player was in idle
            StartCoroutine(WaitForAndDo(0.4f, "AttackEnded"));
        }
    }

    public void Dash(float p_X, float p_Y)
    {
        if (m_mana >= MANACOST_DASH && !m_isDashing && !m_isAttacking)
        {
           // m_mana -= MANACOST_DASH;
            m_isDashing = true;
            // Check if the player is moving or not
            // p_X && p_Y != 0 (Player is moving)
            if (p_X > 0 && p_Y > 0)
            {
                Move(p_X, p_Y, SPEED_DASH);
            }
            // p_X && p_Y == 0 (Player not moving)
            else
            {
                // Determine the direction to dash from the player orientation (currentSide)
                switch (m_currentSide)
                {
                    case Sides.BACK:
                        Move(0, 1, SPEED_DASH);
                        break;

                    case Sides.RIGHT:
                        Move(1, 0, SPEED_DASH);
                        break;

                    case Sides.FRONT:
                        Move(0, -1, SPEED_DASH);
                        break;

                    case Sides.LEFT:
                        Move(-1, 0, SPEED_DASH);
                        break;
                }
            }
        }
        Debug.Log(m_mana);
    }

    private void AttackEnded()
    {
        m_isAttacking = false;
        if (!m_isMoving)
            m_animator.speed = 0;
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
            StartCoroutine(WaitForAndDo(1.0f, "Spawn"));
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

    private void Spawn()
    {
        m_dead = false;
        Debug.Log("IT WORKED!");
    }

    private void DashEnded()
    {
        m_isDashing = false;
        m_rbody2D.velocity = Vector3.zero;
        m_rbody2D.angularVelocity = 0;
        Attack();
    }

    IEnumerator WaitForAndDo(float p_time, string p_TODO)
    {
        yield return new WaitForSeconds(p_time);
        SendMessage(p_TODO);
    }
}
