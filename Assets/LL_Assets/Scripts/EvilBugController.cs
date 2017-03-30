﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvilBugController : MonoBehaviour {

	private List<GameObject> bugPool;//object pool
	[SerializeField] int evilBugCount = 8;//pool size
	[SerializeField] GameObject bug;//evil bugs
	private int indexToUse = 0;//bug to enable

	[SerializeField] int bugsAllowed = 3;//Allow only this many bugs enabled at one time
	private int enabledBugCount = 0;//amonut of currently enabled bugs

	[SerializeField] float instMinTime = 5.0f;//min instantiate time
	[SerializeField] float instMaxTime = 12.0f;//max instantiate time
	private float instantiateTime = 5.0f;//the instantiate time

	private float countTime;//counter to check for instatiation
	private bool canEnable = true;//check if the previous bug has finised being enabled

	//Bug life cycle min and max times
	[SerializeField] float minInTime = 3.0f;
	[SerializeField] float maxInTime = 10.0f;
	[SerializeField] float minAroundTime = 3.0f;
	[SerializeField] float maxAroundTime = 10.0f;
	[SerializeField] float minOutTime = 3.0f;
	[SerializeField] float maxOutTime = 10.0f;

	[SerializeField] Transform[] spawnPoints;//points to spawn the bugs at, 3 points(left, right, top)
	private int spawnIndex = 0;

    [SerializeField]
    private bool pointyBoy = false;

    public bool startSpawn = false;



    //Create pool of evil bugs, instantiate, add to pool, give them a name, disable them
    void Awake()
    {
        bugPool = new List<GameObject>();
		string bugName;
        for (int i = 0; i < evilBugCount; i++)
        {
            if (pointyBoy)
            {
           		bugName = "evilBug_" + "0" + i;
            }
            else
            {
                bugName = "blueBug_" + "0" + i;
            }

            GameObject obj = Instantiate(bug) as GameObject;
            bugPool.Add(obj);
            obj.name = bugName;

            bugPool[i].SetActive(false);
        }

        instantiateTime = instMaxTime;
    }

    void Update ()
	{
		counter ();

		if (countTime >= instantiateTime && startSpawn)
		{
			reset();

			if(canEnable && enabledBugCount < bugsAllowed)
				StartCoroutine(bugLyfe());

		}
	}

	//Reset timeCount and pass new time to instantiate time
	void reset()
	{
		countTime = 0;
		instantiateTime = Random.Range (instMinTime, instMaxTime);
	}

	//Keep track of time
	void counter ()
	{
		countTime += Time.deltaTime;
	}

	//Control bug's lifecycle
	IEnumerator bugLyfe ()
	{
		//set canEnable to false to ensure only one bug is being enabled at a time, increase the index to use, keep track of bugs that have been enabled, increase spawn index
		canEnable = false;
		indexToUse++;
		enabledBugCount++;
		spawnIndex++;

		//Debug.Log("life cycle initiated: " + indexToUse);

		//Roll over index
		if (indexToUse > evilBugCount - 1)
			indexToUse = 0;

		//enable bug
		bugPool [indexToUse].SetActive (true);

		//generate time values for bug's moveIn, moveAronud, and moveOut
		float moveIn = Random.Range (minInTime, maxInTime);
		float moveAround = Random.Range (minAroundTime, maxAroundTime);
		float moveOut = Random.Range (minOutTime, maxOutTime);
		float sum = moveIn + moveAround + moveOut;

		//set the index at which the bug will spawn
		if (spawnIndex > spawnPoints.Length - 1) 
		{
			spawnIndex = 0;
		}

		//give the bug a position to start at
		bugPool[indexToUse].GetComponent<Transform>().position = spawnPoints[spawnIndex].position;

        //Debug.Log("life cycle started: " + indexToUse);
        if (pointyBoy)
        {
            bugPool[indexToUse].GetComponent<EvilBug>().StartBugLyfeCoroutine(moveIn, moveAround, moveOut);

        }
        else {
            bugPool[indexToUse].GetComponent<BlueBug>().StartBugLyfeCoroutine(moveIn, moveAround, moveOut);
        }
		//disable bug afer sum of its life time
		StartCoroutine(endLyfe(indexToUse, sum));

		//another bug can now be instantiated
		canEnable = true;
		yield return null;
	}

	IEnumerator endLyfe(int index, float waitTime)
	{
		//wait until the bug has finished its life cycle
		yield return new WaitForSeconds(waitTime);

		//disable the bug
		bugPool[index].SetActive(false);
		enabledBugCount--;

		//Debug.Log("life cycle over: " + index);
	}
}
