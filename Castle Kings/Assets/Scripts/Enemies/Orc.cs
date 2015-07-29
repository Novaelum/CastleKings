using UnityEngine;
using System.Collections.Generic;

public class Orc : Character {

    struct TargetInfos
    {
        public Transform targetPos;
        public int priorityValue;
    };

    private int m_health;
    Sides m_currentSide;
    Sides m_lastSide;

    private TargetInfos[] m_targetsList = new TargetInfos[8];
    private Rigidbody2D m_rbody2D;
    private float m_speed;
    private bool m_isAttacking;

	// Use this for initialization
	void Start () {
        m_rbody2D = GetComponent<Rigidbody2D>();
        m_health = 100;
        m_speed = 3f;
        InitTargetsList();
        SelectTarget();
	}
	
	// Update is called once per frame
	void Update () {
        if (m_isAttacking)
        {
            AttackTarget();
        }
	}

    // Fill m_targetsList with every player object present in the current scene (max 8)
    private void InitTargetsList()
    {
        int i = 0;
        foreach (Player p in GameObject.FindObjectsOfType(typeof(Player)))
        {
            m_targetsList[i].targetPos = p.transform;
            m_targetsList[i].priorityValue = 0;
            i++;
        }
    }

    private void SelectTarget()
    {
        SetPriorities();
        SortTargetsList();
        m_isAttacking = true;
    }

    private void SetPriorities()
    {

    }

    // Generate a number base on the target's health
    private int HealthPriority(Player p_cTarget) {
        return 0;
    }

    // Generate a number base on the target's range from self position
    private int RangePriority(Player p_cTarget)
    {
        return 0;
    }



    private void AttackTarget() 
    {
        transform.position = Vector2.MoveTowards(transform.position, m_targetsList[0].targetPos.position, Time.deltaTime * m_speed);
    }

    // Sort the target list by priorityValue (bigger number gets first)
    private void SortTargetsList() {
        for (int i = 0; m_targetsList[i].targetPos != null; i++)
        {
            int j = i;
            while (j > 0 && m_targetsList[j - 1].priorityValue < m_targetsList[j].priorityValue)
            {
                TargetInfos temp = m_targetsList[j];
                m_targetsList[j] = m_targetsList[j - 1];
                m_targetsList[j - 1] = temp;
                j--;
            }
        }
    }
}
