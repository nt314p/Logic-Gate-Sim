using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch : Part
{

	private Transform toggle;
	private float currY;
	private float targetY;
	private readonly float offset = 0.015f;
	private readonly float targetTolerance = 0.002f;
	private readonly float toggleSpeed = 0.14f;

	void Awake()
	{
		targetY = -offset;
		currY = targetY;
		toggle = transform.Find("Toggle");
		this.State = false;
		this.Active = true;
	}

	// Update is called once per frame
	void Update()
	{
		toggle.localPosition = Vector3.up * currY; // new Vector3(0, currY, 0); 

		if (Mathf.Abs(currY - targetY) < targetTolerance)
		{
			currY = Mathf.Sign(currY) * offset;
		}
		else
		{
			currY += toggleSpeed * Time.deltaTime * -Mathf.Sign(currY - targetY);
		}
	}

	void OnMouseDown()
	{
		if (Input.GetKey(KeyCode.LeftControl))
		{
			State = !State;
			GetSim().GetCircuit().CalculateStateId(Id);
		}
		else
		{
			Debug.Log("clicked " + this.ToString());
		}
	}

	public override void OnStateUpdate()
	{
		targetY = offset * (State ? 1 : -1);
	}

	public void UpdateState()
	{

	}

	public override void OnSelectUpdate()
	{
		throw new System.NotImplementedException();
	}
}