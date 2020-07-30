using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridController : MonoBehaviour
{
	public static int width = 30;
	public static int height = 30;
	private Material gridMat;
	private GameObject gridPlane;

	void Start()
	{
		gridPlane = this.gameObject;
		gridMat = gridPlane.GetComponent<Renderer>().material;

		gridMat.mainTextureScale = new Vector2(width, height);
		gridPlane.transform.localScale = new Vector3(width, height, 1);
		// gridPlane.transform.position = new Vector3 (width * 0.5f - 0.5f, height * 0.5f - 0.5f, 0);
		gridPlane.transform.position = new Vector3(width / 2f - 0.5f, height / 2f - 0.5f, 0);
	}

	void OnMouseDown()
	{
		Debug.Log("Mouse down on plane!");
		SimulationManager.sim().ClearSelected();
	}

	public void SetWidth(int width)
	{
		GridController.width = width;
	}

	public void SetHeight(int height)
	{
		GridController.height = height;
	}
}