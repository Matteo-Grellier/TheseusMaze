using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour
{

	[SerializeField] private AudioSource getKeySound;
	private bool isKeyTaken = false;
	private float amplitude = 0.3f;
	private float frequency = 1f;
	public Vector3 posOffset = new Vector3 ();
	public Vector3 tempPos = new Vector3 ();

	void Start ()
	{
		posOffset = transform.position;
	}

	private void OnTriggerEnter(Collider other) 
	{
        if (other.gameObject.tag == "Player")
        {
            other.gameObject.GetComponent<Player>().hasKey = true;
			if (!isKeyTaken)
			{
				isKeyTaken = true;
				StartCoroutine(PlaySongThenDestroyKey());
			}
        }
    }

	IEnumerator PlaySongThenDestroyKey()
    {
        getKeySound.Play();
        yield return new WaitWhile (()=> getKeySound.isPlaying);
        Destroy(gameObject);
    }

	void Update ()
	{

		tempPos = posOffset;
		tempPos.y += Mathf.Sin(Time.fixedTime * Mathf.PI * frequency) * amplitude;

		gameObject.transform.Rotate(0, 1, 0);

		transform.position = tempPos;
	}


}
