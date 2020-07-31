using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WireBehavior : MonoBehaviour
{

    private SpriteRenderer _sr;
    private readonly float WIRE_LEN = 1.38f;
	private Wire _wire; // when to assign the reference?


    private void Awake()
    {
		Vector3 direction = (Vector2) _wire.GetOrientation();
		_sr = this.gameObject.GetComponent<SpriteRenderer>();
		this.transform.localScale = new Vector3(WIRE_LEN, WIRE_LEN, 1) + direction * 9 * WIRE_LEN;
		this.transform.position = new Vector3(_wire.Coords.x, _wire.Coords.y, -0.01f) + direction * 0.5f;
	}
	private void Update()
	{
		if (_wire.HasStateUpdate() || _wire.HasStateUpdate()) UpdateColor();
	}

    void OnMouseDown()
	{
		_wire.Selected = !_wire.Selected;
		UpdateColor();
	}

	void OnMouseUp()
	{
		UpdateColor();
		_sr.sortingOrder = 0;
	}

	public void UpdateColor()
	{
		if (_wire.Selected)
		{
			_sr.color = Part.SelectedColor;
			_sr.sortingOrder = 1;
		}
		else
		{
			_sr.color = _wire.State ? Part.ActiveColor : Part.InactiveColor;
		}
	}
}
