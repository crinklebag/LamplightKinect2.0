using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class UI : MonoBehaviour
{
    public bool WinGame { get { return winGame; } set { winGame = value; } }

    public enum IngameMenuStates
    {
        PLAY,
        PAUSE,
        EXIT
    }

    public IngameMenuStates theState;

    public Text scoreText;
    public Text bugsCaughtFG;
    public Text jarsFilledFG;
    public Text totalScoreMulFG;
    public Text totalScoreFG;
    public Text countdown;

    public Image FGOverlay;
    public Image progressBar;
    
    public GameObject am;
    public GameObject exitButtonFG;

    GameController gc;

    [SerializeField] private Color32[] currentColor;
    [SerializeField] GameObject[] jars;
    [SerializeField] float[] intensities;
    [SerializeField] float[] fireflyColorConvert;
    float lerpColorTime = 0;
    float fireflyColorConvertUI;
    float maxLightIntensity = 0.6f;
    float jarsYLerpTime = 0;

    int maxFireflies = 20;
    int score = 0;
    int totalScore = 0;
    int tempScoreCounter = 0;
    [SerializeField] int[] timesFireflyWentHere;

    [SerializeField] bool hasTouchedAtEnd = false;
    [SerializeField] bool winGame = false;
    bool calledCountUpCoroutine = false;
    bool startedGame = false;

    void Awake()
    {
        // countdown = GameObject.Find("Countdown").GetComponent<Text>();
        countdown.gameObject.SetActive(false);
        startedGame = false;

        theState = IngameMenuStates.PLAY;
    }

    // Use this for initialization
    void Start()
    {
        gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();

        StartCoroutine("Countdown");
    }

    // Update is called once per frame
    void Update()
    {
        lerpColorTime = (lerpColorTime += Time.deltaTime) / 1f;

        if (winGame) {
            FinishGame(gc.GetFilledJars());
        }

        if (startedGame) {
            progressBar.fillAmount = am.GetComponent<AudioSource>().time / am.GetComponent<AudioSource>().clip.length;
        }

        if (Input.GetKeyDown(KeyCode.L)) {
            if (!hasTouchedAtEnd)
            {
                Debug.Log("Touched at end using key");
                hasTouchedAtEnd = true;
            }
        }

        scoreText.text = gc.GetBugCount().ToString();
    }

    IEnumerator Countdown()
    {
        yield return new WaitForSecondsRealtime(2);

        for (int i = 3; i >= 0; i--)
        {
            countdown.gameObject.SetActive(true);

            if (i == 0)
            {
                countdown.text = "GO!";
            }
            else
            {
                countdown.text = i.ToString();
            }

            countdown.gameObject.GetComponent<Animator>().Play("CountdownFade", -1, 0.0f);

            yield return new WaitForSecondsRealtime(1);
        }

        countdown.gameObject.SetActive(false);

        startedGame = true;

        gc.StartGameAfterCountdown();
    }

    public void setJarImage(int val)
    {
        for (int i = 0; i < jars.Length; i++)
        {
            jars[i].GetComponent<JarController>().CrackJar(val);
        }
    }

    public void FinishGame(int multiplier)
    {
        score = gc.GetBugCount();
        bugsCaughtFG.text = gc.GetBugCount().ToString();
        jarsFilledFG.text = multiplier.ToString();

        //AE - FGOverlay.gameObject.SetActive(true); - want to move this as the timing seems to be off, as in the level will become dark sometimes before the overlay is up

        if (multiplier > 0)
        {
            totalScoreMulFG.text = score.ToString() + " x " + multiplier.ToString();
            totalScoreFG.text = tempScoreCounter.ToString();
            totalScore = score * multiplier;
        }
        else
        {
            totalScoreMulFG.text = tempScoreCounter.ToString();
            totalScoreFG.text = "";
            totalScore = score;
        }

        if (!calledCountUpCoroutine)
        {
            Debug.Log("Counting Up Score");
            StartCoroutine("CountUpScore");
            calledCountUpCoroutine = true;
        }
    }

    public void SetHasTouchedAtEnd(bool val)
    {
        hasTouchedAtEnd = val;
    }

    IEnumerator CountUpScore()
    {
        yield return new WaitForSecondsRealtime(0.01f);

        while (tempScoreCounter < totalScore)
        {
            if (hasTouchedAtEnd)
            {
                break;
            }

            tempScoreCounter+=10;

            yield return new WaitForSecondsRealtime(0.001f);
        }

        tempScoreCounter = totalScore;
        exitButtonFG.SetActive(true);

        yield return new WaitForSeconds(3.0f);

        gc.ReturnToMenu();
    }

    public void CallPause()
    {
        theState = IngameMenuStates.PAUSE;
    }

    public void showFGOverlay()
    {
        FGOverlay.gameObject.SetActive(true);
    }
}
