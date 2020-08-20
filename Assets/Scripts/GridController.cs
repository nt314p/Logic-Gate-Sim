using UnityEngine;

namespace LogicGateSimulator
{
	public class GridController : MonoBehaviour
	{
		public static int Width = 30;
		public static int Height = 30;
		private Material _gridMaterial;
		private GameObject _gridPlane;

		private void Start()
		{
			_gridPlane = this.gameObject;
			_gridMaterial = _gridPlane.GetComponent<Renderer>().material;

			_gridMaterial.mainTextureScale = new Vector2(Width, Height);
			_gridPlane.transform.localScale = new Vector3(Width, Height, 1);
			_gridPlane.transform.position = new Vector3(Width / 2f - 0.5f, Height / 2f - 0.5f, 0);
		}

		private void OnMouseDown()
		{
			Debug.Log("Mouse down on plane!");
			SimulationManager.Sim().ClearSelected();
		}

		public void SetWidth(int width)
		{
			GridController.Width = width;
		}

		public void SetHeight(int height)
		{
			GridController.Height = height;
		}
	}
}