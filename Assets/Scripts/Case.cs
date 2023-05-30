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
		if (Input.GetMouseButton(0))
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
			switch (objectToPlace)
			{
				case "Wall":
					wallObject.SetActive(true);
					mudObject.SetActive(false);
					trapObject.SetActive(false);
					gravelObject.SetActive(false);
					keyObject.SetActive(false);
					elevatorObject.SetActive(false);
					caseMazeReference.maze.rooms[RoomId].cases[caseId].state = "wall";
					break;
				case "Trap":
					wallObject.SetActive(false);
					mudObject.SetActive(false);
					trapObject.SetActive(true);
					gravelObject.SetActive(false);
					keyObject.SetActive(false);
					elevatorObject.SetActive(false);
					caseMazeReference.maze.rooms[RoomId].cases[caseId].state = "trap";
					break;
				case "Mud":
					wallObject.SetActive(false);
					mudObject.SetActive(true);
					trapObject.SetActive(false);
					gravelObject.SetActive(false);
					keyObject.SetActive(false);
					elevatorObject.SetActive(false);
					caseMazeReference.maze.rooms[RoomId].cases[caseId].state = "mud";
					break;
				case "Gravel":
					wallObject.SetActive(false);
					mudObject.SetActive(false);
					trapObject.SetActive(false);
					gravelObject.SetActive(true);
					keyObject.SetActive(false);
					elevatorObject.SetActive(false);
					caseMazeReference.maze.rooms[RoomId].cases[caseId].state = "gravel";
					break;
				case "Path":
					wallObject.SetActive(false);
					mudObject.SetActive(false);
					trapObject.SetActive(false);
					gravelObject.SetActive(false);
					keyObject.SetActive(false);
					elevatorObject.SetActive(false);
					caseMazeReference.maze.rooms[RoomId].cases[caseId].state = "path";
					break;
				case "Key":
					wallObject.SetActive(false);
					mudObject.SetActive(false);
					trapObject.SetActive(false);
					gravelObject.SetActive(false);
					keyObject.SetActive(true);
					elevatorObject.SetActive(false);
					caseMazeReference.maze.rooms[RoomId].cases[caseId].state = "key";
					break;
				case "Elevator":
					wallObject.SetActive(false);
					mudObject.SetActive(false);
					trapObject.SetActive(false);
					gravelObject.SetActive(false);
					keyObject.SetActive(false);
					elevatorObject.SetActive(true);
					caseMazeReference.maze.rooms[RoomId].cases[caseId].state = "elevator";
					break;
			}
		}
	}
}
