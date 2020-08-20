using UnityEngine;
using UnityEngine.UI;

namespace LogicGateSimulator
{
	public class CanvasController : MonoBehaviour
	{
		[SerializeField] private Text _coordinatesText;
		[SerializeField] private Camera _mainCamera;
	
		private void Update()
		{
			var temp = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
			var coordinates = new Vector2Int(Mathf.RoundToInt(temp.x), Mathf.RoundToInt(temp.y));
			_coordinatesText.text = coordinates.ToString();
		}
	}
}