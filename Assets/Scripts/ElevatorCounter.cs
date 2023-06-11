using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ElevatorCounter : MonoBehaviour
{
    [SerializeField] private TMP_Text floorText;
    [SerializeField] private int endFloor = -10;
    [SerializeField] private AudioSource clicSound;
    [SerializeField] private AudioSource bellSound;
    private int floor;
    private string floorStr = "Floor : ";

    private void Start()
    {
        floor = 0;
        StartCoroutine(CountdownFloors());
    }

    private IEnumerator CountdownFloors()
    {
        while((-floor) > endFloor)
        {
            yield return new WaitForSeconds(2);
            floor++;
            floorText.text = floorStr + "-" + floor;
            clicSound.Play();
            yield return null;
        }

        if(-floor == endFloor)
            bellSound.Play();
    }
}
