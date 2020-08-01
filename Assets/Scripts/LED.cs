using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LED : Part
{
	private static readonly bool IS_ACTIVE_PART = false;
	
	public LED() : base(IS_ACTIVE_PART)
    {
		this.State = false;
    }	
}