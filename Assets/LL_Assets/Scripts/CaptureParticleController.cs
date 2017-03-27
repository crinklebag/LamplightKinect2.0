using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaptureParticleController : MonoBehaviour {

    GameController gameController;

    Vector3 endMarker;
    Vector3 startMarker;

    // Need all of these sent over from the firefly ecxcept for the speed
    float startTime = 0;
    float speed = 1;
    float journeyLength = 0;
    int randJar;

    bool canMove = false;

	// Use this for initialization
	void Start () {
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
	}
	
	// Update is called once per frame
	void Update () {
        if (canMove)
        {
            // This should also be moved int the particle - instead o destroying the object just turn it off
            this.transform.position = Vector3.MoveTowards(this.transform.position, endMarker, 10 * Time.deltaTime);

            if ((this.transform.position.x < endMarker.x - 0.01f || this.transform.position.x > endMarker.x + 0.01f) &&
                (this.transform.position.y < endMarker.y - 0.01f || this.transform.position.y > endMarker.y + 0.01f))
            {
                float distCovered = (Time.time - startTime) * speed;
                float fracJourney = distCovered / journeyLength;
                this.transform.position = Vector3.MoveTowards(this.transform.position, endMarker, fracJourney);
            }
            else
            {
                this.GetComponent<ParticleSystem>().Stop();
            }
        }
    }

    public void SetDestination(Vector3 newStartMarker, Vector3 newEndMarker) {
        startTime = Time.time;
        startMarker = newStartMarker;
        endMarker = newEndMarker;
        journeyLength = Vector3.Distance(startMarker, endMarker);
        canMove = true;
    }
}
