﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FunFactController : MonoBehaviour
{
    private string[] facts;
    private int numOfFacts = 5;

    [SerializeField]
    Text factsText;
    [SerializeField]
    float waitTime = 7.0f;


    private void Awake()
    {
        facts = new string[numOfFacts];

        facts[0] = "There are more than 2,000 species of fireflies";
        facts[1] = "In some places at some times, fireflies synchronize their flashing";
        facts[2] = "A fireflies light is the most efficient light in the world";
        facts[3] = "Firefly larvae are carnivorous and particularly enjoy snails";
        facts[4] = "Observing fireflies in your backyard can help scientists learn more about these insects and why they're dissapearing";

    }

    // Use this for initialization
    void Start()
    {
        factsText.text = facts[Random.Range(0, facts.Length)];

        StartCoroutine(Wait());
    }

    // Update is called once per frame
    void Update()
    {

    }

    private IEnumerator Wait()
    {
        yield return new WaitForSeconds(waitTime);
        this.GetComponent<SceneLoad>().LoadScene("MainMenu");
    }
}
