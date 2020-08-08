using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WireBehavior : PartBehavior
{

    private static readonly float WIRE_LEN = 1.38f;

    private void Awake()
    {
		Vector3 direction = (Vector2) ((Wire) PartObject).GetOrientation();
		SpriteRenderer = this.gameObject.GetComponent<SpriteRenderer>();
		this.transform.localScale = new Vector3(WIRE_LEN, WIRE_LEN, 1) + direction * 9 * WIRE_LEN;
		this.transform.position = new Vector3(PartObject.Coordinates.x, PartObject.Coordinates.y, -0.01f) + direction * 0.5f;
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
