using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireFlicker : MonoBehaviour {
    
    [SerializeField] Light fireLight;
    float currentIntensity = 1;

	// Use this for initialization
	void Start () {
        StartCoroutine(FlickerLight());
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    IEnumerator FlickerLight()
    {

        yield return new WaitForSeconds(0.05f);

        int rand = Random.Range(0, 2);
        float tempIntensity = 0;
        
        switch (rand)
        {
            case 0:
                tempIntensity = currentIntensity - 0.15f;
                break;
            case 1:
                tempIntensity = currentIntensity;
                break;
            case 2:
                tempIntensity = currentIntensity + 0.15f;
                break;
        }

        fireLight.intensity = tempIntensity;
        StartCoroutine(FlickerLight());
    }
}
