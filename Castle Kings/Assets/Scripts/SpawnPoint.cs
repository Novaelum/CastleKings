using UnityEngine;
using System.Collections;

public class SpawnPoint : MonoBehaviour {

    public Player m_Owner;

	// Use this for initialization
	void Start () {
         
	}

    void OnTriggerEnter2D(Collider2D objHit)
    {
        if (objHit.gameObject.tag == "Player")
        {
            // Makes sure if the player is it's owner and if he carry the princess
            if ((m_Owner.Equals(objHit.gameObject.GetComponent<Player>())) && objHit.gameObject.GetComponent<Player>().m_carryPrincess) {
                objHit.gameObject.SendMessage("Score");
            }
            else
            {
                Debug.Log("You don't belong here!");
            }
                 
        }
    }

    public void SetOwner()
    {
        // TODO: Set the owner when called
    }
	
}
