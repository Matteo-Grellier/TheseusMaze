using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour
{
    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag == "Player")
        {
            other.gameObject.GetComponent<Player>().hasKey = true;
            Destroy(gameObject);
        }
    }

	public float amplitude = 100f;
	public float frequency = 1f;
	public Vector3 posOffset = new Vector3 ();
	public Vector3 tempPos = new Vector3 ();

	void Start ()
	{
		posOffset = transform.position;
	}

	void Update ()
	{

		tempPos = posOffset;
		tempPos.y += Mathf.Sin(Time.fixedTime * Mathf.PI * frequency) * amplitude;

		gameObject.transform.Rotate(0, 1, 0);

		transform.position = tempPos;
	}


}
