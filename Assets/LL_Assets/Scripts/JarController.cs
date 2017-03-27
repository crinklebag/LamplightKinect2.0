using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JarController : MonoBehaviour {

    GameController gameController;

    [SerializeField] Sprite[] jarCracks;
    [SerializeField] SpriteRenderer jarImage;
    [SerializeField] GameObject bugGlow;
    [SerializeField] Light jarHalo;

    int damage = 0;
    int maxDamage = 3;

    int maxBugs = 20;
    int bugCount = 0;
    int timesFilled = 0;
    bool isFull = false;

    // Use this for initialization
    void Start() {
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
    }

    // Update is called once per frame
    void Update() {

    }

    public void AddBug() {
        bugCount++;
        bugGlow.SetActive(true);
        jarHalo.intensity = bugCount * 0.05f;

        if (bugCount == maxBugs) {
            isFull = true;
            timesFilled++;
            gameController.FillJar();
            Debug.Log("Jar Filled");
        }
    }

    public void RemoveBug() {
        bugCount--;
        jarHalo.intensity = bugCount * 0.5f;

        if (bugCount == 0) {
            bugGlow.SetActive(false);
            jarHalo.intensity = 0;
        }
    }

    public void ResetJar() {
        jarImage.sprite = jarCracks[0];
        jarHalo.intensity = 0;
        bugGlow.SetActive(false);
        bugCount = 0;
    }

    public void CrackJar(int index) {
        jarImage.sprite = jarCracks[index];
    }

    public bool IsFull() {
        return isFull;
    }
}
