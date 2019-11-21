using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class LookAt : MonoBehaviour {

	public Transform target;
	
	void LateUpdate () {
		if(target != null)
			transform.LookAt(target);
	}
}
