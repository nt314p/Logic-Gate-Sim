using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasController : MonoBehaviour
{
	[SerializeField] private Text _coordinatesText;
	[SerializeField] private GameObject _cameraGameObject;
	
	void Update()
	{
		var temp = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		var coordinates = new Vector2Int(Mathf.RoundToInt(temp.x), Mathf.RoundToInt(temp.y));
		_coordinatesText.text = coordinates.ToString();
	}
}