using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JarController : MonoBehaviour {

    GameController gameController;

    [SerializeField] Sprite[] jarCracks;
    [SerializeField] SpriteRenderer jarImage;
    [SerializeField] GameObject bugGlow;
    [SerializeField] GameObject jarHalo;

    int damage = 0;
    int maxDamage = 3;

    float alphaIncreaseValue = 0;
    float currentAlpha = 0;

    int maxBugs = 20;
    int bugCount = 0;
    int timesFilled = 0;
    bool isFull = false;

    // Use this for initialization
    void Start() {
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        alphaIncreaseValue = (30 / 20) * 0.01f;
        StartCoroutine(FlickerLight());
    }

    // Update is called once per frame
    void Update() {

    }

    public void AddBug() {
        bugCount++;
        bugGlow.SetActive(true);
        currentAlpha = currentAlpha + alphaIncreaseValue;
        Color newColor = new Color(jarHalo.GetComponent<SpriteRenderer>().color.r, jarHalo.GetComponent<SpriteRenderer>().color.g, jarHalo.GetComponent<SpriteRenderer>().color.b, currentAlpha);
        jarHalo.GetComponent<SpriteRenderer>().color = newColor;
        bugGlow.GetComponent<SpriteRenderer>().color = new Color(1.0f, 1.0f, 1.0f, bugCount * 0.1f); 
            // jarHalo.intensity = bugCount * 0.05f;
		this.GetComponent<JarPulse> ().SetPulse (true);
        if (bugCount == maxBugs) {
            isFull = true;
            timesFilled++;
            gameController.FillJar();
         //   Debug.Log("Jar Filled");
        }
    }

    public void RemoveBug() {
        bugCount--;
        currentAlpha = currentAlpha - alphaIncreaseValue;
        Color newColor = new Color(jarHalo.GetComponent<SpriteRenderer>().color.r, jarHalo.GetComponent<SpriteRenderer>().color.g, jarHalo.GetComponent<SpriteRenderer>().color.b, currentAlpha);
        jarHalo.GetComponent<SpriteRenderer>().color = newColor;
        // jarHalo.intensity = bugCount * 0.1f;

        if (bugCount == 0) {
            bugGlow.SetActive(false);
            jarHalo.GetComponent<SpriteRenderer>().color = Color.clear;
            currentAlpha = 0;
        }
    }

    public void ResetJar() {
        jarImage.sprite = jarCracks[0];
        jarHalo.GetComponent<SpriteRenderer>().color = Color.clear;
        currentAlpha = 0;
        bugGlow.SetActive(false);
        bugCount = 0;
    }

    public void CrackJar(int index) {
        jarImage.sprite = jarCracks[index];
    }

    public bool IsFull() {
        return isFull;
    }

    IEnumerator FlickerLight()
    {

        yield return new WaitForSeconds(0.1f);

        int rand = Random.Range(0, 2);
        float tempAlpha = 0;

        if (currentAlpha > 0) {
            switch (rand) {
                case 0:
                    tempAlpha = currentAlpha - 0.05f;
                    break;
                case 1:
                    tempAlpha = currentAlpha;
                    break;
                case 2:
                    tempAlpha = currentAlpha + 0.05f;
                    break;
            }
        }

        jarHalo.GetComponent<SpriteRenderer>().color = new Color(jarHalo.GetComponent<SpriteRenderer>().color.r, jarHalo.GetComponent<SpriteRenderer>().color.g, jarHalo.GetComponent<SpriteRenderer>().color.b, tempAlpha);
        StartCoroutine(FlickerLight());
    }
}
