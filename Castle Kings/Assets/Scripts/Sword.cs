using UnityEngine;
using System.Collections;

public class Sword : MonoBehaviour {

    public Player m_Owner;

	// Use this for initialization
	void Start () {
       
	}

    void OnTriggerEnter2D(Collider2D objHit) {
        if (objHit.gameObject.tag == "Player" && !objHit.gameObject.GetComponent<Player>().IsDead())
        {
            objHit.gameObject.SendMessage("Killed");
            m_Owner.IncreaseScore(50);
        }
    }
	
	
}
