using System;
using LogicGateSimulator.Parts;
using UnityEngine;

namespace LogicGateSimulator.PartBehaviors
{
	public class WireBehavior : PartBehavior
	{
		private const float WireLen = 1.3888f;
		public override Type PartType => typeof(Wire);

		private void Start()
		{
			if (PartObject == null) return;
			SetupWire();
		}

		private void SetupWire()
		{
			Vector3 direction = (Vector2) ((Wire) PartObject).Orientation;
			var wireTransform = this.transform;
			wireTransform.localScale = new Vector3(WireLen, WireLen, 1) + direction * (9 * WireLen);
			wireTransform.position = new Vector3(PartObject.Coordinates.x, PartObject.Coordinates.y, -0.01f) + direction * 0.5f;
			UpdateColor();
		}

		protected override void OnPartObjectChanged()
		{
			SetupWire();
		}

		private void OnMouseUp()
		{
			UpdateColor();
			SpriteRenderer.sortingOrder = 0;
		}

		protected override void UpdateColor()
		{
			if (this.Selected)
			{
				SpriteRenderer.color = this.SelectedColor;
				SpriteRenderer.sortingOrder = 1;
			}
			else
			{
				if (PartObject != null) SpriteRenderer.color = PartObject.State ? this.ActiveColor : this.InactiveColor;
			}
		}
	}
}
