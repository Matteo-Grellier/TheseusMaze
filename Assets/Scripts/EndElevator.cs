using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndElevator : MonoBehaviour
{
    [SerializeField] private GameObject door;
    [SerializeField] private GameObject btnPrefabReference;
    [SerializeField] private float doorUpY = 3;
    [SerializeField] private float doorSpeed = 0.001f;
    private bool isDoorOpen = false; 
    public bool isPlayerIn = false;
    
    void Awake()
    {
        //GameObject btn = Instantiate(btnPrefabReference, new Vector3(transform.position.x + 0.55f, transform.position.y + 1.2f, transform.position.z), Quaternion.Euler(new Vector3(transform.rotation.x, 90, transform.rotation.z)));
        //btn.GetComponent<EndButton>().endElevatorReference = this;
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.name == "Player" && !isPlayerIn)
        {
            if(other.GetComponent<Player>().hasKey == true)
                isDoorOpen = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if(other.name == "Player")
        {
            isDoorOpen = false;
        }
    }

    void FixedUpdate()
    {
        // Debug.Log("<color=purple>door.transform.position.y =</color> " + door.transform.position.y);
        if(!isPlayerIn && isDoorOpen && door.transform.position.y < doorUpY)
            door.transform.position = new Vector3(door.transform.position.x, door.transform.position.y + doorSpeed, door.transform.position.z); 
        else if (!isDoorOpen && door.transform.position.y > 1.111405f || isPlayerIn && door.transform.position.y > 1.111405f)
            door.transform.position = new Vector3(door.transform.position.x, door.transform.position.y - doorSpeed, door.transform.position.z); 
        
        if (isPlayerIn && door.transform.position.y <= 1.5f)
        {
            Debug.Log("WIN");
            GameManager.instance.Win();
        }
    
    }
}
