using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kinect = Windows.Kinect;

public class followhand : BodySourceView {


	public GameObject bodypart;
	public Kinect.JointType TrackedJoint;
	private BodySourceManager bodyManager;
	private Kinect.Body[] bodies;

	bool lostSkeleton = false;

	[SerializeField]public float speed;

	[SerializeField]public float distance;

	[SerializeField]GameObject panel;

	public Vector3 temp;
	public int ID;
	// Use this for initialization
	void Start () {
		bodyManager = BodySourceManager.GetComponent<BodySourceManager> ();
	}

	// Update is called once per frame
	void Update () {
		if (bodyManager == null)
		{
			// Debug.Log ("Body Manager Null");
			return;
		}
		bodies = bodyManager.GetData ();
		if (bodies == null)
		{
			// NOTIFY PLAYER SKELETON IS BEING BUILT
			Debug.Log ("bodies null");
			lostSkeleton = true;
			//StartCoroutine(BuildSkeleton());
			return;
		}
		foreach (var body in bodies) 
		{
			
			if (body == null)
			{
				
				continue;
			}
			if (body.IsTracked) {
				
				//panel.SetActive (false);
				//lostSkeleton = false;
				Debug.Log ("Built it");
				if (body.Joints [TrackedJoint].Position.Z < distance) {
					var pos = body.Joints [TrackedJoint].Position;


					temp = new Vector3 (pos.X, pos.Y, 99);
					ID = (int)body.TrackingId;
					//var rot = body.Joints [TrackedJoint].Position;
					//gameObject.transform.position = new Vector3 (pos.X, pos.Y,0)*Time.time*speed;
					//gameObject.transform.position = Vector3.MoveTowards(transform.position, new Vector3(pos.X,pos.Y,0), Time.deltaTime * speed * 1000);
					//gameObject.transform.rotation = new Quaternion (rot.X, rot.Y,0,0);
					Debug.Log (ID + "WOOT");

					if ((int)body.TrackingId == ID) {
						SmoothMove ();
						lostSkeleton = false;
					} 


				}

			} else {
				//lostSkeleton = true;
				//StartCoroutine (BuildSkeleton ());
				bodies = null;
			}
		}


	}

	void SmoothMove()
	{
		
		this.transform.position = Vector3.Lerp (this.transform.position, new Vector3(temp.x * speed, temp.y * speed, 99), Time.smoothDeltaTime);
		// Debug.Log ("Moving");
	}

	IEnumerator BuildSkeleton(){
		yield return new WaitForSeconds (0.1f);
		if (lostSkeleton == true) {
			// Keep notifying the player
			Debug.Log ("no skeleton , buildin it");

			panel.SetActive (true);
		} else 
		{
			panel.SetActive (false);
			StopCoroutine (BuildSkeleton ());
		}
			
	}
}
