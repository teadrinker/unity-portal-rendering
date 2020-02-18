using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnityXRInputAdapter : MonoBehaviour {
	public bool LeftTrigger;
	public bool RightTrigger;
	public bool LeftGrab;
	public bool RightGrab;



	Vector2 RightThumbstickPos;
	void Update () {


		LeftTrigger = UnityEngine.Input.GetAxis("Axis9") > 0.2f;
		RightTrigger = UnityEngine.Input.GetAxis("Axis10") > 0.2f;
		LeftGrab = UnityEngine.Input.GetAxis("Axis11") > 0.2f;
		RightGrab = UnityEngine.Input.GetAxis("Axis12") > 0.2f;

		RightThumbstickPos = new Vector2(
				UnityEngine.Input.GetAxis("Axis5"),
				UnityEngine.Input.GetAxis("Axis4"));



	}
}
