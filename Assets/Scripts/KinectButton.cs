using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KinectButton : MonoBehaviour {

    bool fade = false;
    Color fadeToColor = new Color(0, 0, 0, 0);

    [SerializeField]
    MenuFly menufly;

    [SerializeField]
    GameObject text;

    [SerializeField]
    Text buttonText;

    [SerializeField]
    string buttontext;

    // Use this for initialization
    void Start () {
        menufly = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<MenuFly>();
	}
	
	// Update is called once per frame
	public void setFade(bool state) {
        fade = state;
	}


    void Update()
    {
        if (fade) {
           this.gameObject.GetComponent<Image>().color = Color.Lerp(this.gameObject.GetComponent<Image>().color, fadeToColor, Time.deltaTime);
           

          buttonText.text = buttontext;
          Debug.Log("Reset name to: " + buttontext);
        }
    }
    
   
}
