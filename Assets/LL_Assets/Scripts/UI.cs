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

    [SerializeField]
    private GameObject WavePanel;
    [SerializeField]
    private GameObject WaveText;
    [SerializeField]
    private float desiredPanelHeight = -500.0f;
    [SerializeField]
    private float wavePanelMoveSpeed = 10.0f;
    [SerializeField]
    private float panelUpTime = 1.0f;
    private bool isMovingWave = false;
    private int WavePanelMoveCount = 0;
    private int WaveCount = 0;

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
    public int totalScore = 0;
    int tempScoreCounter = 0;
    [SerializeField] int[] timesFireflyWentHere;

    [SerializeField] bool hasTouchedAtEnd = false;
    [SerializeField] bool winGame = false;
    bool calledCountUpCoroutine = false;
    bool startedGame = false;

    private float timeRemaining;
    private bool startedProgressBarFlash = false;
    private bool startedFinalCountdown = false;

    [SerializeField]
    private float FinalScoreFinishWait = 5.0f;

    [SerializeField]
    private float progressFlashSpeed = 5.0f;

	[SerializeField] private Sprite[] finalPanelBgs;
	[SerializeField] private GameObject[] finalScoreObjs;

	private bool calledFadeCoroutine = false;


    void Awake()
    {
        // countdown = GameObject.Find("Countdown").GetComponent<Text>();
        countdown.gameObject.SetActive(false);
        startedGame = false;

        theState = IngameMenuStates.PLAY;
    }
	void Start()
    {
		gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
	}

    // Use this for initialization
    public void StartGame()
    {
       //gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();

        StartCoroutine("Countdown");

    }

    // Update is called once per frame
    void Update()
    {
        lerpColorTime = (lerpColorTime += Time.deltaTime) / 1f;

        if (winGame) {
            FinishGame(gc.GetFilledJars());
        }

        if (startedGame)
        {
            timeRemaining = am.GetComponent<AudioSource>().clip.length - am.GetComponent<AudioSource>().time;

            progressBarUpdate();
            endGameUpdate();
        }

        if (Input.GetKeyDown(KeyCode.L)) {
            if (!hasTouchedAtEnd)
            {
                Debug.Log("Touched at end using key");
                hasTouchedAtEnd = true;
            }
        }

        scoreText.text = gc.GetBugCount().ToString();
        wavePanelUpdate();
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

		setFinalImage ();

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
		PlayerPrefs.SetInt ("Player Score", totalScore);
    }

    public void SetHasTouchedAtEnd(bool val)
    {
        hasTouchedAtEnd = val;
    }

    IEnumerator CountUpScore()
    {
        yield return new WaitForSecondsRealtime(0.001f);

        while (tempScoreCounter < totalScore)
        {
            if (hasTouchedAtEnd)
            {
                break;
            }

			if (tempScoreCounter < totalScore * 0.5f) 
			{
				tempScoreCounter+=20;
			}
			else if (tempScoreCounter < totalScore * 0.75f) 
			{
				tempScoreCounter+=10;
			}
			else
			{
				tempScoreCounter++;
			}

            yield return new WaitForSecondsRealtime(0.001f);
        }

        tempScoreCounter = totalScore;
        exitButtonFG.SetActive(true);

		yield return new WaitForSeconds(FinalScoreFinishWait);

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

    public void wavePanelUpdate()
    {
        WaveText.GetComponent<Text>().text = "WAVE " + WaveCount.ToString();

        if (WavePanelMoveCount > 0 && !isMovingWave)
        {
            WavePanelMoveCount--;
            isMovingWave = true;
            StartCoroutine(moveWavePanel());
        }
    }

    IEnumerator moveWavePanel()
    {
        float tempY = WavePanel.transform.localPosition.y;

        while (tempY < (desiredPanelHeight - 0.01f))
        {
            tempY = Mathf.MoveTowards(tempY, desiredPanelHeight, Time.deltaTime * wavePanelMoveSpeed);
            WavePanel.transform.localPosition = new Vector3(WavePanel.transform.localPosition.x, tempY, WavePanel.transform.localPosition.z);
            yield return null;
        }

        yield return new WaitForSeconds(panelUpTime);

        while (tempY > -749.99f)
        {
            tempY = Mathf.MoveTowards(tempY, -750.0f, Time.deltaTime * wavePanelMoveSpeed);
            WavePanel.transform.localPosition = new Vector3(WavePanel.transform.localPosition.x, tempY, WavePanel.transform.localPosition.z);
            yield return null;
        }

        isMovingWave = false;
        yield return null;
    }

    public void incWaveCount()
    {
        WaveCount++;
        WavePanelMoveCount++;
    }

    void progressBarUpdate()
    {
        progressBar.fillAmount = am.GetComponent<AudioSource>().time / am.GetComponent<AudioSource>().clip.length;

        //Debug.Log("t: " + timeRemaining);

        if (timeRemaining <= 10.0f && !startedProgressBarFlash)
        {
            startedProgressBarFlash = true;
            StartCoroutine(flashProgressBar());
        }

        if (progressBar.fillAmount >= 0.999f)
        {
            winGame = true;
            gc.FinishGame();
        }
    }

    void endGameUpdate()
    {
        if (timeRemaining <= 5.0f && !startedFinalCountdown)
        {
            startedFinalCountdown = true;
            StartCoroutine(FinishGameCountdown());
        }
    }

    IEnumerator flashProgressBar()
    {
        float temp = progressBar.color.b;

        while (temp > 0.0f)
        {
            temp = Mathf.MoveTowards(temp, 0.0f, Time.deltaTime * progressFlashSpeed);
            progressBar.color = new Color(progressBar.color.r, temp, temp);
            yield return null;
        }

        yield return new WaitForSeconds(0.1f);

        while (temp < 1.0f)
        {
            temp = Mathf.MoveTowards(temp, 1.0f, Time.deltaTime * progressFlashSpeed);
            progressBar.color = new Color(progressBar.color.r, temp, temp);
            yield return null;
        }

        startedProgressBarFlash = false;
        yield return null;
    }

    IEnumerator FinishGameCountdown()
    {
        countdown.gameObject.SetActive(true);
        float intervalTime = timeRemaining / 3.0f;

        for (int i = 3; i > 0; i--)
        {
            countdown.text = i.ToString();

            countdown.gameObject.GetComponent<Animator>().Play("CountdownFade", -1, 0.0f);

            yield return new WaitForSeconds(intervalTime);
        }

        countdown.gameObject.SetActive(false);
        yield return null;
    }

	void setFinalImage()
	{
		switch (PlayerPrefs.GetInt("bgNumber"))
		{
		case 1:
			FGOverlay.sprite = finalPanelBgs[0];
			break;
		case 2:
			FGOverlay.sprite = finalPanelBgs[1];
			break;
		case 3:
			FGOverlay.sprite = finalPanelBgs[2];
			break;
		case 4:
			FGOverlay.sprite = finalPanelBgs[3];
			break;
		}
	}

	IEnumerator fadeInScorePanel()
	{
		for(int i = 0; i < finalScoreObjs.Length; i++)
		{
			finalScoreObjs[i].SetActive(false);
		}

		FGOverlay.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
		FGOverlay.gameObject.SetActive(true);

		float tempA = FGOverlay.color.a;

		while(tempA < 0.999f)
		{
			tempA = Mathf.MoveTowards(tempA, 1.0f, Time.deltaTime * 5.0f);

			FGOverlay.color = new Color(1.0f, 1.0f, 1.0f, tempA);

			yield return null;
		}

		for(int i = 0; i < finalScoreObjs.Length; i++)
		{
			finalScoreObjs[i].SetActive(true);
		}

		yield return null;
	}
}
