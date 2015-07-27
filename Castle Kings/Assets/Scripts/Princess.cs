using UnityEngine;
using System.Collections;

public class Princess : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}

    void OnTriggerEnter2D(Collider2D objHit)
    {
        if (objHit.gameObject.tag == "Player")
        {
            objHit.gameObject.SendMessage("TakePrincess");
            Destroy(this.gameObject);
        }
    }
}
