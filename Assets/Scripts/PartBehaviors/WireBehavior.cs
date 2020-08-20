using LogicGateSimulator.Parts;
using UnityEngine;

namespace LogicGateSimulator.PartBehaviors
{
	public class WireBehavior : PartBehavior
	{
		private const float WireLen = 1.38f;

		private void Awake()
		{
			if (PartObject == null) return;
			Vector3 direction = (Vector2) ((Wire) PartObject).Orientation;
			var wireTransform = this.transform;
			wireTransform.localScale = new Vector3(WireLen, WireLen, 1) + direction * (9 * WireLen);
			wireTransform.position = new Vector3(PartObject.Coordinates.x, PartObject.Coordinates.y, -0.01f) + direction * 0.5f;
		}

		public override void OnPartObjectChanged()
		{
			Awake();
		}

		private void OnMouseUp()
		{
			UpdateColor();
			SpriteRenderer.sortingOrder = 0;
		}

		public override void UpdateColor()
		{
			if (this.Selected)
			{
				SpriteRenderer.color = this.SelectedColor;
				SpriteRenderer.sortingOrder = 1;
			}
			else
			{
				SpriteRenderer.color = PartObject.State ? this.ActiveColor : this.InactiveColor;
			}
		}
	}
}
