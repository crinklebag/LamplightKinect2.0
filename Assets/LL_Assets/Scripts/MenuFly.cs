using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MenuFly : MonoBehaviour
{

    public float glowlevel = 0.0f;
    public float glowlevel2 = 0.0f;
    [SerializeField]
    public float playtime = 3;
    public float wait = 2.0f;
    [SerializeField]
    Text countdown;
    Color tempcolor2;
    Image obj;
    bool glowing = false;
    // Use this for initialization

    [SerializeField]
    GameObject menuController;



    void OnTriggerStay2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Button"))
        {
            // Debug.Log("Glowing");
            // glowing = true;
            Glow(col.gameObject);
            obj = col.gameObject.GetComponent<Image>();
        }


    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Button"))
        {
            col.gameObject.GetComponent<KinectButton>().setFade(false);


        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Button"))
        {
            playtime = 3;
            countdown.gameObject.SetActive(false);
            // glowlevel = Mathf.Lerp(glowlevel, 0.0f, Time.deltaTime);
            glowing = false;
            col.gameObject.GetComponent<KinectButton>().setFade(true);
        }
    }
    // Update is called once per frame
    void Update()
    {


        /*if (!glowing)
        {
            glowlevel2 = Mathf.Lerp(glowlevel, 0.0f, Time.deltaTime);
        }
        Color tempColor = this.GetComponent<SpriteRenderer> ().color;
		tempColor.a = glowlevel;
		
		this.GetComponent<SpriteRenderer> ().color = tempColor;
        if (obj == null)
        {
            return;
        }
        obj.color = tempColor;*/

        //ALL FOR MENUS STUFFS

        if (playtime <= 0)
        {
            Debug.Log("here");
            if (obj.name == "Kinect Forest Button")
            {
                Debug.Log("here");
                menuController.GetComponent<MainMenuController>().ButtonClick(2, "Forest");

            }
            if (obj.name == "Start Game")
            {
                menuController.GetComponent<MainMenuController>().ButtonClick(1, "");
                Debug.Log("whatshappening");
            }

            if (obj.name == "SongKinectButton1")
            {
                menuController.GetComponent<MainMenuController>().ButtonClick(3, "Seasons Change");
            }

            if (obj.name == "SongKinectButton2")
            {
                menuController.GetComponent<MainMenuController>().ButtonClick(3, "Get Free");
            }

            if (obj.name == "SongKinectButton3")
            {
                menuController.GetComponent<MainMenuController>().ButtonClick(3, "Dream Giver");
            }

            if (obj.name == "SongKinectButton4")
            {
                menuController.GetComponent<MainMenuController>().ButtonClick(3, "Spirit Speaker");
            }
        }

    }

    void Glow(GameObject col)
    {
        //tempcolor2.a = glowlevel2;

        //glowlevel += 0.5f;
        glowlevel = Mathf.Lerp(glowlevel, 1.0f, Time.deltaTime);
        wait -= Time.deltaTime;
        glowlevel2 = Mathf.Lerp(glowlevel2, 1.0f, Time.deltaTime);
        wait -= Time.deltaTime;

        Color tempColor = this.GetComponent<SpriteRenderer>().color;
        tempColor.a = glowlevel;
        col.gameObject.GetComponent<Image>().color = tempColor;

        if (wait <= 0)
        {
            countdown.gameObject.SetActive(true);
            playtime -= Time.deltaTime;
            int Tempplaytime = (int)playtime + 1;
            // countdown.text = Tempplaytime.ToString ();
            if (Tempplaytime >= 0)
            {

                col.gameObject.GetComponentInChildren<Text>().text = Tempplaytime.ToString();
            }
            // Debug.Log ("Load Scene");

            if (Tempplaytime == 0)
            {
                countdown.text = "Play";

            }

        }

    }

}
