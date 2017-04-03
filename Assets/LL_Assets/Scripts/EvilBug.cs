﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvilBug : MonoBehaviour {

	private GameController gameController;

	private Vector3 destination;
	private Vector3 randomPosition;

	private float minMoveX = -10.0f;
    private float maxMoveX = 10.0f;
	private float minMoveY = -5.5f;
    private float maxMoveY = 5.5f;

    private float timeCount = 0.0f;

    //LookAt 2D
	private Vector3 normTarget;
	private float angle;
	private Quaternion rot;


    [SerializeField] GameObject glow;
    [SerializeField] GameObject sprite;
    [SerializeField] GameObject hitParticle;
	[SerializeField] GameObject notification;
	[SerializeField] GameObject redParticles;

    [SerializeField] float speed = 1.25f;
    [SerializeField] float rotSpeed = 5.0f;


    private bool beenHit = false;

    AudioSFX aSFX;

	//Notification variables
	[SerializeField] private float margin = 1.0f; //Margin outside the bounds for when the bug's notification should turn off
	[SerializeField] private float notificationScale = 4.0f; //Not affected scale
	[SerializeField] private float notificationScaleMul = 1.5f; //Multiply the scale by this when flashing on beat
	[SerializeField] private float notificationScaleSpeed = 5.0f; //On beat scale speed
	[SerializeField] private float notificationActiveScaleSpeed = 15.0f; //Activate/Deactivate scale speed

	//Notification bools
	private bool isNotificationOff = false;
	private bool isNotificationOn = false;
	private bool canFlash = false;

    void Awake ()
	{
		gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
		aSFX = GameObject.Find("SFXController").GetComponent<AudioSFX>();
	}

    void Update ()
	{
		CountTime();
		RandomPosition();
		lookAtPosition();
		showNotification();
		flashNotificationCheck();
	}

	//Call to start life cycle
	public void StartBugLyfeCoroutine (float inT, float arT, float outT)
	{
		StartCoroutine(BugLyfe(inT, arT, outT));
		redParticles.SetActive (true);
	}

	//Controls bug's life cycle
	IEnumerator BugLyfe (float inTime, float aroundTime, float outTime)
	{
		ResetCountTime();
		yield return StartCoroutine(MoveIn(inTime));
		ResetCountTime();
		yield return StartCoroutine(MoveAround(aroundTime));
		ResetCountTime();
		yield return StartCoroutine(MoveOut(outTime));

		yield return null;
	}

	//Move in from off the screen for a give time
	IEnumerator MoveIn (float t)
	{				
		destination = new Vector3(0.0f, 0.0f, -1.0f);

		while(timeCount < t)
		{
			this.transform.position = Vector3.MoveTowards(this.transform.position, destination, Time.deltaTime * speed);
			
			yield return null;
		}
		yield return null;
	}

	//move around for a give time
	IEnumerator MoveAround (float t)
	{
		while(timeCount < t)
		{
			destination = randomPosition;

			this.transform.position = Vector3.MoveTowards(this.transform.position, destination, Time.deltaTime * speed);

			yield return null;
		}
		yield return null;
	}

	//move off the screen for a given time
	IEnumerator MoveOut (float t)
	{
		destination = new Vector3(this.transform.position.x, -10.0f, -1.0f);
		while(timeCount < t)
		{
			this.transform.position = Vector3.MoveTowards(this.transform.position, destination, Time.deltaTime * speed);
			
			yield return null;
		}
		yield return null;
	}

	//Set randomPosition to a random position within the give range when audioManager returns a beat
	void RandomPosition ()
	{
		if (AudioManager.beatCheckHalf)
		{
			float randY = Random.Range(minMoveY, maxMoveY);
            float randX = Random.Range(minMoveX, maxMoveX);

            randomPosition = new Vector3(randX, randY, -1.0f);
		}
	}

	//Constantly force the bug to face the in the direction of its destination
	void lookAtPosition ()
	{
		normTarget = (new Vector2(destination.x, destination.y) - new Vector2(this.transform.position.x, this.transform.position.y)).normalized;
		angle = Mathf.Atan2(normTarget.y, normTarget.x)*Mathf.Rad2Deg;

		rot = new Quaternion();
		rot.eulerAngles = new Vector3(0,0,angle-90);
		this.transform.rotation = Quaternion.Slerp(this.transform.rotation, rot, rotSpeed * Time.deltaTime);
	}

	//Increase count by delta time
	void CountTime ()
	{
		timeCount += Time.deltaTime;
	}

	//Call before each part of the life cycle to ensure life cycle runs the correct amonut of time
	void ResetCountTime()
	{
		timeCount = 0.0f;
	}

     void OnTriggerEnter2D(Collider2D other)
    {

        if (!beenHit)
        {
            beenHit = true;

            if (other.gameObject.CompareTag("JarTop"))
            {
                aSFX.playDodo();
                endLyfe();
                gameController.CrackJar();
                // gameController.makeLotsOfBugs();
                // gameController.GetComponent<VibrationController>().Vibrate(); ?????
            }
        }
    }
		
    //turn sprites off and turn particle effect on
    void endLyfe()
    {
        glow.SetActive(false);
        sprite.SetActive(false);
		redParticles.SetActive (false);

       GameObject RedParticle = Instantiate(hitParticle) as GameObject;
       RedParticle.transform.position = this.transform.position;
    }

	//Check if object is visible
	//Check which side were on
	//Set notification position & set active
	void showNotification()
	{
		float posX = this.transform.position.x;
		float posY = this.transform.position.y;

		if(posX <= (minMoveX - margin) && posY <= maxMoveY)//left side
		{
			//Debug.Log("Left Side");

			notification.transform.position = new Vector3(minMoveX - 0.5f, this.transform.position.y, this.transform.position.z);
			if(!isNotificationOn)
			{
				isNotificationOn = true;
				StartCoroutine(turnOnNotification());
			}
		}
		else if (posX >= (maxMoveX + margin)&& posY <= maxMoveY)//right side
		{
			//Debug.Log("Right Side");

			notification.transform.position = new Vector3(maxMoveX + 0.5f, this.transform.position.y, this.transform.position.z);
			if(!isNotificationOn)
			{
				isNotificationOn = true;
				StartCoroutine(turnOnNotification());
			}
		}
		else if (posY >= (maxMoveY + margin) && posX >= minMoveX && posX <= maxMoveX)//top side
		{
			//Debug.Log("Top Side");

			notification.transform.position = new Vector3(this.transform.position.x, maxMoveY + 0.5f, this.transform.position.z);
			if(!isNotificationOn)
			{
				isNotificationOn = true;
				StartCoroutine(turnOnNotification());
			}
		}
		else if (posX <= (minMoveX - margin) && posX <= maxMoveX && posY >= (maxMoveY + margin))//top left corner
		{
			//Debug.Log("Top Left Side");

			notification.transform.position = new Vector3(minMoveX - 0.5f, maxMoveY + 0.5f, this.transform.position.z);
			if(!isNotificationOn)
			{
				isNotificationOn = true;
				StartCoroutine(turnOnNotification());
			}
		}
		else if (posX >= (maxMoveX + margin) && posX >= minMoveX && posY >= (maxMoveY + margin))//top right corner
		{
			//Debug.Log("Top Right Side");

			notification.transform.position = new Vector3(maxMoveX + 0.5f, maxMoveY + 0.5f, this.transform.position.z);
			if(!isNotificationOn)
			{
				isNotificationOn = true;
				StartCoroutine(turnOnNotification());
			}
		}
		else
		{
			//Debug.Log("Visible!");
			if(!isNotificationOff)
			{
				isNotificationOff = true;
				StartCoroutine(turnOffNotification());
			}

		}
	}

	//Check if notification is active and if it isn't already flashing
	//flash on beat, start flash notification coroutine
	void flashNotificationCheck()
	{
		if(notification.activeSelf && canFlash)
		{
			if(AudioManager.beatCheck)
			{
				StartCoroutine(flashNotification());
			}
		}
	}

	//Set local variables
	//While notification size is less than the desired size move towards the desired size
	//Then move back to its original size
	IEnumerator flashNotification()
	{
		float currSize = notification.transform.localScale.x;
		float desiredSize = notification.transform.localScale.x * notificationScaleMul;
		float tempSize = notification.transform.localScale.x;

		while(tempSize < desiredSize)
		{
			tempSize = Mathf.MoveTowards(tempSize, desiredSize, Time.deltaTime * notificationScaleSpeed);
			notification.transform.localScale = new Vector3(tempSize, tempSize, 1.0f);
			yield return null;
		}

		while(tempSize > currSize)
		{
			tempSize = Mathf.MoveTowards(tempSize, currSize, Time.deltaTime * notificationScaleSpeed);
			notification.transform.localScale = new Vector3(tempSize, tempSize, 1.0f);
			yield return null;
		}

		yield return null;
	}

	//Set notification obj active
	//Increase notification scale until it has reached the proper size
	//Allow notification to flash by setting bool
	IEnumerator turnOnNotification()
	{
		notification.SetActive(true);

		float tempSize = notification.transform.localScale.x;

		while(tempSize <= notificationScale - 0.05f)
		{
			tempSize = Mathf.MoveTowards(tempSize, notificationScale, Time.deltaTime * notificationActiveScaleSpeed);
			notification.transform.localScale = new Vector3(tempSize, tempSize, 1.0f);
			yield return null;
		}

		canFlash = true;
		yield return null;
	}

	//Scale obj notification size down until it is almost 0
	//Set notification object not active
	IEnumerator turnOffNotification()
	{
		float tempSize = notification.transform.localScale.x;

		while(tempSize > 0.05f)
		{
			tempSize = Mathf.MoveTowards(tempSize, 0.05f, Time.deltaTime * notificationActiveScaleSpeed);
			notification.transform.localScale = new Vector3(tempSize, tempSize, 1.0f);
			yield return null;
		}

		notification.SetActive(false);
		yield return null;
	}
}
