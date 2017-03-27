using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FireFly : MonoBehaviour
{

    public GameObject fireflySparklePrefab;
    [SerializeField] GameObject captureParticles;

    GameController gameController;
    [SerializeField] GameObject[] jars;
    [SerializeField] GameObject image;
    [SerializeField] GameObject glow;
    [SerializeField] GameObject particles;
    [SerializeField] float speed = 0.05f;
    [SerializeField] float rotSpeed = 2.5f;
    [SerializeField] GameObject destination;
    [SerializeField] GameObject spotLight;
    [SerializeField] ParticleSystem fireflyShimmer;

    public bool isOn;
    [SerializeField] bool caught = false;
    float minMoveX = -10.0f;
    float maxMoveX = 10.0f;
    float minMoveY = -5.5f;
    float maxMoveY = 5.5f;
    Vector2 newPos;

    //look at 2d
    private Vector3 normTarget;
    private float angle;
    private Quaternion rot;

    [SerializeField] float freqMoveThresh;

    private float timeTillNewPosition;

    Vector3 startMarker = Vector3.zero;
    float startTime = 0;
    float journeyLength = 0;
    bool setDest = false;
    bool canBounceGlow = false;
    bool increaseGlow = false;

    int randJar = 0;

    // Use this for initialization
    void Start()
    {
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        
        StartCoroutine("RandomPosition");
        StartCoroutine(ChangeState());

        jars = GameObject.FindGameObjectsWithTag("jar");

        image.SetActive(false);
    }


    public void startFireflyLife()
    {
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!caught)
        {
            moveBug();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {

        if (other.gameObject.CompareTag("JarTop") && isOn) {

            GameObject fireflySparkle = GameObject.Instantiate(fireflySparklePrefab);
            SetDestination();
            //Debug.Log("Hit Jar Top Trigger Enter");
            caught = true;
            // Turn off bug and glow
            image.SetActive(false);
            glow.SetActive(false);
            spotLight.SetActive(false);
            fireflyShimmer.Stop();

            // Tell the gc you caught a bug
            gameController.CatchBug();

            // Calculate the journey length and get the start pos - this should be done in the particle or sent to the particle??
            startMarker = this.transform.position;
            startTime = Time.time;
            
            // Instantiate particles and send them on a journey
            GameObject newCaptureParticles = GameObject.Instantiate(captureParticles, this.transform.position, Quaternion.identity);
            newCaptureParticles.GetComponent<CaptureParticleController>().SetDestination(startMarker, destination.transform.position);

            // Add a bug to the randomly selected jar
            destination.GetComponent<JarController>().AddBug();

            // Destroy the bug
            Destroy(this.gameObject);
        }
    }

    void SetDestination()
    {
        setDest = true;
        randJar = gameController.WhichJar();
        destination = jars[randJar];
    }

    void BounceGlow() {
        if (increaseGlow) {

        } else {

        }
    }

    //Check if band frequency is above the threshold, if so give it a new target position
    IEnumerator ChoosePath()
    {
        yield return new WaitForSeconds(0.1f);

        int band = glow.GetComponent<Flicker>()._band;
        float bandFreq = AudioPeer._audioBandBuffer[band];

        if (bandFreq >= freqMoveThresh)
        {
            float randY = Random.Range(minMoveY, maxMoveY);
            float randX = Random.Range(minMoveX, maxMoveX);

            newPos = new Vector2(randX, randY);
        }
        StartCoroutine(ChoosePath());
    }

    IEnumerator RandomPosition()
    {
        if (!caught)
        {
            timeTillNewPosition = Random.Range(0.1f, 3.5f);

            yield return new WaitForSecondsRealtime(timeTillNewPosition);

            float randY = Random.Range(minMoveY, maxMoveY);
            float randX = Random.Range(minMoveX, maxMoveX);

            newPos = new Vector2(randX, randY);

            StartCoroutine(RandomPosition());
        }
    }

    //Move to and face the target position
    void moveBug()
    {
        transform.position = Vector2.MoveTowards(this.transform.position, newPos, speed * Time.deltaTime);

        //Look at 2D
        normTarget = (newPos - new Vector2(this.transform.position.x, this.transform.position.y)).normalized;
        angle = Mathf.Atan2(normTarget.y, normTarget.x) * Mathf.Rad2Deg;

        rot = new Quaternion();
        rot.eulerAngles = new Vector3(0, 0, angle - 90);
        this.transform.rotation = Quaternion.Slerp(this.transform.rotation, rot, rotSpeed * Time.deltaTime);
    }

    // If the bug's light is on turn the bug image off, if it's off turn the bug image on
    IEnumerator ChangeState()
    {
        if (isOn)
        {
            // Debug.Log("Turning Off");
            image.SetActive(false);
            // Turn off Light
            spotLight.gameObject.SetActive(true);
            // Stop Particles
            fireflyShimmer.Play();
        }
        else
        {
            // Debug.Log("Turning On");
            image.SetActive(true);
            // Turn on light
            spotLight.gameObject.SetActive(false);
            // Start particles
            fireflyShimmer.Stop();
        }
        yield return new WaitForSeconds(0.1f);
        StartCoroutine(ChangeState());
    }

    public bool IsOn()
    {
        return isOn;
    }
}
