using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class StartButtonScalar : MonoBehaviour {

	[SerializeField]float min;
	[SerializeField]float max;
	[SerializeField] public float speed;
	[SerializeField]Text text;




	// Use this for initialization
	void Start () {
		
		StartCoroutine (ScaleText ());
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}


	IEnumerator ScaleText(){
		float tempScaleX = text.transform.localScale.x;

		while(tempScaleX < max - 0.01f){
			tempScaleX = Mathf.MoveTowards (tempScaleX, max, Time.deltaTime * speed);
			text.transform.localScale = new Vector3 (tempScaleX, tempScaleX, 1.0f);
			yield return null;
		}

		while (tempScaleX > min + 0.01f) {
			tempScaleX = Mathf.MoveTowards (tempScaleX, min, Time.deltaTime * speed);
			text.transform.localScale = new Vector3 (tempScaleX, tempScaleX, 1.0f);
			yield return null;
		}
		 

		StartCoroutine (ScaleText ());
	}
}

