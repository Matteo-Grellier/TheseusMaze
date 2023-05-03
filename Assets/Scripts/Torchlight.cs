using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Torchlight : MonoBehaviour
{

    private bool isLightUp = true;
    private float blinkTimeLeft = 0;

    // Start is called before the first frame update
    void Start()
    {
        blinkTimeLeft = Random.Range(10, 15);
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(blinkTimeLeft);
        if (Input.GetKeyDown(KeyCode.F))
        {
            isLightUp = !isLightUp;
            // Turn on the light
            GetComponent<Light>().enabled = isLightUp;
        }

        if (isLightUp)
        {
            if (blinkTimeLeft <= 0)
            {
                InvokeRepeating("blink", 0.1f, 0.01f);
                Invoke("stopBlink", 0.4f);
                blinkTimeLeft = Random.Range(0, 10);

            }
            else
            {
                blinkTimeLeft -= Time.deltaTime;
            }
        }
    }

    void blink()
    {
        GetComponent<Light>().enabled = !GetComponent<Light>().enabled;
    }

    void stopBlink()
    {
        CancelInvoke("blink");
        GetComponent<Light>().enabled = true;
    }
}
