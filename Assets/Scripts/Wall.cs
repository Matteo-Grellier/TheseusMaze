using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class Wall : MonoBehaviour
{
    [Tooltip("reference to the parent case")] [SerializeField] private Case parentcase;

	private ToolsPanel toolsPanel;

    // when intercept a click event, transmit it to the case
    private void OnMouseDown()
    {
        if (Input.GetMouseButtonDown(0))
        {
            parentcase.CaseLeftClicked(toolsPanel.objectToPlace);
        }
    }

	private void OnMouseOver()
	{
		if (Input.GetMouseButton(0))
		{
			parentcase.CaseLeftClicked(toolsPanel.objectToPlace);
		}
	}

	private void Awake()
	{
		if (SceneManager.GetActiveScene().name == "EditScene")
		{
			toolsPanel = GameObject.Find("ToolsPanel").GetComponent<ToolsPanel>();
		}
	}
}
