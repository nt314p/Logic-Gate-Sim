using UnityEngine;
using UnityEngine.UI;

namespace LogicGateSimulator
{
	public class CanvasController : MonoBehaviour
	{
		[SerializeField] private Text coordinatesText;
		[SerializeField] private Camera mainCamera;
	
		private void Update()
		{
			var temp = mainCamera.ScreenToWorldPoint(Input.mousePosition);
			var coordinates = new Vector2Int(Mathf.RoundToInt(temp.x), Mathf.RoundToInt(temp.y));
			coordinatesText.text = coordinates.ToString();
		}
	}
}