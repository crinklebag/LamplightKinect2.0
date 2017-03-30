using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroAnimController : MonoBehaviour {

	[SerializeField]GameObject introAnim;
	[SerializeField]GameObject menuFly;
	[SerializeField]GameObject canvas;


	// Use this for initialization
	void Start () {
		
		StartCoroutine(StopAnim());
		menuFly.SetActive (false);
		canvas.SetActive (false);
		}
	


	IEnumerator StopAnim()
	{
		yield return new WaitForSecondsRealtime(3);
		introAnim.SetActive (false);
		menuFly.SetActive (true);
		canvas.SetActive (true);
	}
}
