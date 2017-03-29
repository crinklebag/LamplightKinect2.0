using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public Transform[] SpawnPoints { get { return spawnPoints; } }

    public Texture[] netStates;
    public SkinnedMeshRenderer net;

    UI uiController;
    AudioManager audioManager;
    LightController lightController;

    private AudioSFX aSFX;

    GameObject player;
    GameObject JarTopCollider;
    [SerializeField] GameObject fireflyPrefab;
    [SerializeField] GameObject fireflyLightPrefab;
    [SerializeField] GameObject pauseButton;

    public static int[] bandFrequencies;
    [SerializeField] float[] bounds;
    int maxBugs = 5;
    int jarDamageLimit = 3;
    int maxBugCount = 20;
    [SerializeField] int jarCurrentDamage = 0;
    [SerializeField] bool hitAlready = false;
    [SerializeField] bool startGameInEditor = false;
    [SerializeField] bool stopDoingThis = false;
    
    [SerializeField] Transform[] spawnPoints;//For the fireflies
    private int spawnIndex = 0;//Which spawn point are we using

    int caughtBugs = 0;
    int activeBugs = 0;
    int filledJars = 0;

    // Use this for initialization
    void Start() {
        Initialize();
    }

    void Initialize() {
        // Set up the audio frequencies for fireflies
        bandFrequencies = new int[10];

        // Get Game Object refernces
        uiController = GameObject.FindGameObjectWithTag("UIController").GetComponent<UI>();
        player = GameObject.FindGameObjectWithTag("Player");
        JarTopCollider = GameObject.FindGameObjectWithTag("JarTop");
        audioManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();
        lightController = GameObject.FindGameObjectWithTag("LightManager").GetComponent<LightController>();
        
        // Disable the player script
        player.GetComponent<Net>().enabled = false;
    }

    public void StartGameAfterCountdown()
    {
        // IF we are playing ini editor manually set a song to play
        if (Application.isEditor) {
            PlayerPrefs.SetString("sceneNumber", "Dream Giver");
        }

        // Enable the player script
        player.GetComponent<Net>().enabled = true;

        aSFX = GameObject.Find("SFXController").GetComponent<AudioSFX>();

        audioManager.gameObject.GetComponent<AudioSource>().Stop();
        StartCoroutine(audioManager.StartAudio());

        lightController.SetGame();

        InitializeBugs();
    }

    void InstantiateBug()
    {
        for (int j = 0; j < maxBugs; j++)
        {
            spawnIndex = Random.Range(0, spawnPoints.Length);
            GameObject newBug = Instantiate(fireflyPrefab, spawnPoints[spawnIndex].position, Quaternion.identity) as GameObject; //Instantitate at random spawn point

            newBug.GetComponent<FireFly>().startFireflyLife();
            newBug.GetComponentInChildren<Flicker>()._band = bandFrequencies[j];
            activeBugs++;
        }
    }

    IEnumerator InstantiateNewBugs()
    {
        for (int j = 0; j < maxBugs; j++)
        {
            spawnIndex = Random.Range(0, spawnPoints.Length);
            GameObject newBug = Instantiate(fireflyPrefab, spawnPoints[spawnIndex].position, Quaternion.identity) as GameObject;

            newBug.GetComponent<FireFly>().startFireflyLife();
            newBug.GetComponentInChildren<Flicker>()._band = bandFrequencies[j];
            activeBugs++;

            float randWaitTime = Random.Range(0.4f, 1.5f);
            yield return new WaitForSeconds(randWaitTime);
        }
    }

    void InitializeBugs()
    {
        for (int i = 0; i < maxBugs; i++)
        {
            float randX = Random.Range(bounds[2], bounds[3]);
            float randY = Random.Range(bounds[0], bounds[1]);
            //Vector3 randPos = new Vector3(randX, randY, -0.5f);
            spawnIndex = Random.Range(0, spawnPoints.Length);
            GameObject newBug = Instantiate(fireflyPrefab, spawnPoints[spawnIndex].position, Quaternion.identity) as GameObject;

            newBug.GetComponent<FireFly>().startFireflyLife();

            //Assign each bug a frequency band, band range 0-5
            if (i > 5)
            {
                int tempBand = i - 6;
                newBug.GetComponentInChildren<Flicker>()._band = tempBand;
            }
            else
            {
                newBug.GetComponentInChildren<Flicker>()._band = i;
            }

            bandFrequencies[i] = newBug.GetComponentInChildren<Flicker>()._band;
            
        }

        StartCoroutine("CheckToMakeNewFirefly");
    }

    public void CatchBug( )
    {
        caughtBugs++;
        activeBugs--;
    }

    public void FinishGame()
    {
        stopDoingThis = true;
        player.GetComponent<followhand>().SetEndGame(true);
        StopAllCoroutines();

        //show end game overlay and start end game audio
        uiController.showFGOverlay();
        StartCoroutine(GameObject.Find("AudioManager").GetComponent<AudioManager>().EndGame());//Perform audio tasks at end of game (fade out current audio, fade in end game audio)

        Destroy(player.GetComponent<Net>());
        Destroy(player.GetComponent<BoxCollider2D>());
        Destroy(JarTopCollider);

        uiController.WinGame = true;
        pauseButton.SetActive(false);
    }

    public void CrackJar()
    {
        if (hitAlready)
        {
            return;
        }

        jarCurrentDamage++;

        if (jarCurrentDamage < jarDamageLimit)
        {

            StartCoroutine("PlayerDragonflyCooldown");
        }

        if (jarCurrentDamage <= jarDamageLimit)
        {
            Debug.Log("Changing net mat");
            uiController.setJarImage(jarCurrentDamage);
            net.material.SetTexture("_MainTex", netStates[jarCurrentDamage]);
            player.GetComponent<followhand>().FlashJar();
        }


        if (jarCurrentDamage == jarDamageLimit)
        {
            FinishGame();
        }
    }

    IEnumerator CheckToMakeNewFirefly()
    {
        if (!stopDoingThis)
        {
            if (activeBugs < maxBugCount)
            {

                // InstantiateBug();
                StartCoroutine(InstantiateNewBugs());
            }

            //Debug.Log("Went to CheckToMakeNewBug, seconds: " + seconds);
            int seconds = Random.Range(5, 10);
            yield return new WaitForSecondsRealtime(seconds);

            StartCoroutine("CheckToMakeNewFirefly");
        }
    }

    IEnumerator PlayerDragonflyCooldown()
    {
        hitAlready = true;

        StartCoroutine(player.GetComponent<followhand>().FlashJar());

        yield return new WaitForSecondsRealtime(2.5f);

        hitAlready = false;
    }

    public int WhichJar()
    {
        return filledJars % 5;
    }

    public void makeLotsOfBugs()
    {
        uiController.incWaveCount();

        for (int j = 0; j < 10; j++)
        {
            spawnIndex = Random.Range(0, spawnPoints.Length);
            GameObject newBug = Instantiate(fireflyPrefab, spawnPoints[spawnIndex].position, Quaternion.identity) as GameObject; //Instantitate at random spawn point
            // Debug.Log("poopcrapfuck");
            // newBug.GetComponent<FireFly>().startFireflyLife();
            newBug.GetComponentInChildren<Flicker>()._band = bandFrequencies[j];
            activeBugs++;
        }

    }

   
    //Call on exit button press
    //fade audio back out, change to fun facts loading scene
    public void ReturnToMenu()
    {
        SceneManager.LoadScene("LoadingFacts");

        return;
    }

    public void FillJar() {
        filledJars++;
    }

    public int GetFilledJars() {
        return filledJars;
    }

    public int GetBugCount() {
        return caughtBugs;
    }
}
