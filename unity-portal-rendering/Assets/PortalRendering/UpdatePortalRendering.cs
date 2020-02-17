// Martin Eklund, Space Plunge, 2020
// https://github.com/teadrinker/unity-portal-rendering

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class UpdatePortalRendering : MonoBehaviour {

	public float portalSize = 0.4f;

	[Range(0f, 100f)]
	public float portalSizeMarginPercentage = 30f; // need margin unless we only see from the front

	[Space()]

	public GameObject portalTargetTransform;
	public GameObject portalViewPointSource;
	public GameObject portalQuad;
	public Material realtimeProjectionMat;
	public Camera portalRenderCam;
	public Camera exportCam;

	public Vector2 exportCamAspect = new Vector2(16f, 9f); // 16/9


	void Awake() {
		// these are not VR cameras!
		portalRenderCam.stereoTargetEye = StereoTargetEyeMask.None; 
		exportCam.stereoTargetEye = StereoTargetEyeMask.None; 
	}

	void Update() {

		// Generate camera parameters as if there was no portalTargetTransform
		// (portal is just showing what it behind it, so it is "invisible")
		portalRenderCam.transform.position = portalViewPointSource.transform.position;
		portalRenderCam.transform.LookAt(portalQuad.transform.position);

		var portalDir = Vector3.back;

		var distanceToPortal = (portalViewPointSource.transform.position - portalQuad.transform.position).magnitude;
		
		var radius = (portalSize * (1f + portalSizeMarginPercentage/100f)) / 2f;

		// relationship between fov and camera distance ( *2f because radius is half fov)
		var fov = Mathf.Atan2(radius, distanceToPortal) * Mathf.Rad2Deg * 2f;

		portalRenderCam.fieldOfView = fov;
		exportCam.orthographicSize = portalSize * 0.5f * Mathf.Clamp(exportCamAspect.y / exportCamAspect.x, 0f, 1f);
		portalQuad.transform.localScale = Vector3.one * portalSize;

		RealtimeUVProjectionTexture(portalRenderCam, realtimeProjectionMat);

		if(portalTargetTransform != null) {

			// now get the transform difference between portal and portal target
			var portalM = gameObject.transform.localToWorldMatrix; 
			var portalTargetM = portalTargetTransform.transform.localToWorldMatrix; 
			var diff = portalTargetM * portalM.inverse;

			// apply transform difference to portal renderer camera
			var camM = portalRenderCam.transform.localToWorldMatrix; 
			var newCamM = diff * camM;
			portalRenderCam.transform.SetPositionAndRotation(newCamM.GetColumn(3), newCamM.rotation);
		}
	}


	void RealtimeUVProjectionTexture(Camera MatrixSource, Material DestinationMaterial) {
		if(MatrixSource != null && DestinationMaterial != null) {
			var P = MatrixSource.projectionMatrix;
			//P = GL.GetGPUProjectionMatrix(P, true);
			Matrix4x4 V = MatrixSource.worldToCameraMatrix;

			Matrix4x4 VP = P * V;
			DestinationMaterial.SetMatrix("_UVProjMatrix", VP);
		}
	}

}
