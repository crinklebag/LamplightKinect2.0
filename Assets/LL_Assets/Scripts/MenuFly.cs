using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

/*
 
    TODO:
    Load the scene - cannot test until it is set up
    Figure out why the layer mask is being an idiot - Turn off other layer mask object and BGs once selected
    Set up Player Boundaries
     
*/

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

    [SerializeField] GameObject menuController;
    [SerializeField] GameObject backyardLayerMask;
    [SerializeField] GameObject deepWoodsLayerMask;
    [SerializeField] GameObject watrefallLayerMask;
    [SerializeField] GameObject beachLayerMask;

	[SerializeField] Text text;
    void OnTriggerStay2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Button"))
        {
            // Debug.Log("Glowing");
            // glowing = true;
            Glow(col.gameObject);
            obj = col.gameObject.GetComponent<Image>();
			col.gameObject.GetComponent<StartButtonScalar> ().speed = 0.0f;
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Button"))
        {
            col.gameObject.GetComponent<KinectButton>().setFade(false);
			col.gameObject.GetComponent<StartButtonScalar> ().speed = 0.0f;
			if (col.gameObject.name == "Start Game") {
				col.gameObject.GetComponent<StartButtonScalar> ().speed = 0.0f;
				//text.gameObject.transform.localScale = new Vector3 (1.0f, 1.0f, 0.0f);
			}
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
			col.gameObject.GetComponent<StartButtonScalar> ().speed = 0.075f;
			if (col.gameObject.name == "Start Game") {
			col.gameObject.GetComponent<StartButtonScalar> ().speed = 0.075f;
				//text.gameObject.transform.localScale = new Vector3 (1.0f, 1.0f, 0.0f);
			}

        }
    }
    // Update is called once per frame
    void Update()
    {

        //ALL FOR MENUS STUFFS
        if (playtime <= 0)
        {
            if (obj.name == "Start Game") {
                menuController.GetComponent<MainMenuController>().ButtonClick(1, "");
            }

            SelectLevel();
            SelectSong();
        }
    }

    void SelectSong() {
        // Song Selection
        if (obj.name == "Seasons Change")
        {
            menuController.GetComponent<MainMenuController>().ButtonClick(3, "Seasons Change");
            Debug.Log("Seasons change song choose");
        }

        if (obj.name == "Get Free")
        {
            menuController.GetComponent<MainMenuController>().ButtonClick(3, "Get Free");
        }

        if (obj.name == "Dream Giver")
        {
            menuController.GetComponent<MainMenuController>().ButtonClick(3, "Dream Giver");
        }

        if (obj.name == "Spirit Speaker")
        {
            menuController.GetComponent<MainMenuController>().ButtonClick(3, "Spirit Speaker");
        }
    }

    void SelectLevel() {
        // Background Selection
        if (obj.name == "Backyard Button")
        {
            menuController.GetComponent<MainMenuController>().ButtonClick(2, "Backyard");
            backyardLayerMask.GetComponent<StretchUIMask>().ChooseLocation();
        }
        if (obj.name == "Deep Forest Button")
        {
            menuController.GetComponent<MainMenuController>().ButtonClick(2, "Deep Forest");
            deepWoodsLayerMask.GetComponent<StretchUIMask>().ChooseLocation();
        }
        if (obj.name == "Waterfall Button")
        {
            menuController.GetComponent<MainMenuController>().ButtonClick(2, "Waterfall");
            watrefallLayerMask.GetComponent<StretchUIMask>().ChooseLocation();
        }
        if (obj.name == "Beach Button")
        {
            menuController.GetComponent<MainMenuController>().ButtonClick(2, "Beach");
            beachLayerMask.GetComponent<StretchUIMask>().ChooseLocation();
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
               // countdown.text = "Play";
                if (SceneManager.GetActiveScene().buildIndex != 2)
                {
                    GameObject.FindGameObjectWithTag("UIController").GetComponent<NewCredits>().fadeToMenu();
                    //SceneManager.LoadScene("MainMenu");
                }

            }

        }
    }
}
