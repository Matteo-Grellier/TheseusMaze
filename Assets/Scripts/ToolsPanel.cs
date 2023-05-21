using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ToolsPanel : MonoBehaviour
{

	public string objectToPlace;

	public bool movePanel = false;

	public TMP_Text textShowButton;

	private void Update()
	{
		if (movePanel)
		{
			transform.position = Vector3.Lerp(transform.position, new Vector3(800, 46.49997f, 0), Time.deltaTime * 5);
			// Find GameObject
			textShowButton.text = ">";


		}
		else
		{
			transform.position = Vector3.Lerp(transform.position, new Vector3(1370, 46.49997f, 0), Time.deltaTime * 5);
			textShowButton.text = "<";


		}

	}

	public void BtnMovePanel()
	{
		movePanel = !movePanel;
	}

	public void BtnSetObjectToWall()
	{
		objectToPlace = "Wall";
	}

	public void BtnSetObjectToPath()
	{
		objectToPlace = "Path";
	}

	public void BtnSetObjectToTrap()
	{
		objectToPlace = "Trap";
	}

	public void BtnSetObjectToMud()
	{
		objectToPlace = "Mud";
	}

	public void BtnSetObjectToGravel()
	{
		objectToPlace = "Gravel";
	}

	public void BtnSetObjectToElevator()
	{
		objectToPlace = "Elevator";
	}

	public void BtnSetObjectToKey()
	{
		objectToPlace = "Key";
	}
}
