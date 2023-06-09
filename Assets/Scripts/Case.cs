using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;

public class Case : MonoBehaviour
{
	[Header("Mandatory References")]
	public GameObject wallObject;
	public GameObject gravelObject;
	public GameObject mudObject;
	public GameObject trapObject;
	public GameObject elevatorObject;
	public GameObject keyObject;
	public GameObject debugCase;
	public GameObject previewObject;

	public int caseId;
	public int RoomId;

	public Maze caseMazeReference; // set in the Case
	private GameManager gameManager;

	private ToolsPanel toolsPanel;

	private void Awake()
	{
		if (SceneManager.GetActiveScene().name == "EditScene")
		{
			toolsPanel = GameObject.Find("ToolsPanel").GetComponent<ToolsPanel>();
		}
		gameManager = GameManager.instance;
	}

	public void CaseClicked()
	{
		if (gameManager.isEditMode == true)
		{
			wallObject.SetActive(!wallObject.gameObject.activeInHierarchy);
		}
	}

	// detect if the click is down when passing on the case
	private void OnMouseOver()
	{
		if (Input.GetMouseButton(0) && GameManager.instance.isEditMode == true)
		{
			if (!EventSystem.current.IsPointerOverGameObject())
			{
				CaseLeftClicked(toolsPanel.objectToPlace);
			}
		}
	}

	public void CaseLeftClicked(string objectToPlace)
	{
		print(objectToPlace);
		if (gameManager.isEditMode == true)
		{
			wallObject.SetActive(false);
			mudObject.SetActive(false);
			trapObject.SetActive(false);
			gravelObject.SetActive(false);
			keyObject.SetActive(false);
			elevatorObject.SetActive(false);
			switch (objectToPlace)
			{

				case "Wall":
					wallObject.SetActive(true);
					caseMazeReference.maze.rooms[RoomId].cases[caseId].state = "wall";
					break;
				case "Trap":
					trapObject.SetActive(true);
					caseMazeReference.maze.rooms[RoomId].cases[caseId].state = "trap";
					break;
				case "Mud":
					mudObject.SetActive(true);
					caseMazeReference.maze.rooms[RoomId].cases[caseId].state = "mud";
					break;
				case "Gravel":
					gravelObject.SetActive(true);
					caseMazeReference.maze.rooms[RoomId].cases[caseId].state = "gravel";
					break;
				case "Key":
					keyObject.SetActive(true);
					caseMazeReference.maze.rooms[RoomId].cases[caseId].state = "key";
					break;
				case "Elevator":
					elevatorObject.SetActive(true);
					caseMazeReference.maze.rooms[RoomId].cases[caseId].state = "elevator";
					break;
			}
		}
	}
}
