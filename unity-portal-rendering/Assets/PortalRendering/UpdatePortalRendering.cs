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
	public GameObject portalViewPointSource;
	public Vector2 exportCamAspect = new Vector2(16f, 9f); // 16/9
	
	[Space()]

	public GameObject portalTargetTransform;
	public GameObject portalQuad;
	public Material realtimeProjectionMat;
	public Camera portalRenderCam;
	public Camera exportCam;



	void Awake() {
		// these are not VR cameras!
		portalRenderCam.stereoTargetEye = StereoTargetEyeMask.None; 
		exportCam.stereoTargetEye = StereoTargetEyeMask.None; 
	}

	void Update() {
		if(portalViewPointSource == null) {
			Debug.LogWarning("UpdatePortalRendering: You need to assign portalViewPointSource!");
			return;
		}

		// Generate camera parameters as if there was no portalTargetTransform
		// (portal is just showing what it behind it, so it is "invisible")
		var viewPos = portalViewPointSource.transform.position;
		portalRenderCam.transform.position = viewPos;
		portalRenderCam.transform.LookAt(portalQuad.transform.position);


		//var distanceToPortal = (portalViewPointSource.transform.position - portalQuad.transform.position).magnitude;
		//var radius = (portalSize * (1f + portalSizeMarginPercentage/100f)) / 2f;
		// relationship between fov and camera distance ( *2f because radius is half fov)
		//var fov = Mathf.Atan2(radius, distanceToPortal) * Mathf.Rad2Deg * 2f;
		
		// fov calculation using angles between corners and center is more precise
		// var s = portalSize * (1f + portalSizeMarginPercentage/100f);
		// var corner1 = transform.TransformPoint(new Vector3(  s/2f,  s/2f, 0f));
		// var corner2 = transform.TransformPoint(new Vector3( -s/2f, -s/2f, 0f));
		// var corner3 = transform.TransformPoint(new Vector3(  s/2f, -s/2f, 0f));
		// var corner4 = transform.TransformPoint(new Vector3( -s/2f,  s/2f, 0f));
		// var towardsPortalCenter = (transform.position - viewPos).normalized;
		// var halfDiagonalFov =                        Vector3.Angle(towardsPortalCenter, (corner1 - viewPos).normalized);
		// halfDiagonalFov = Mathf.Max(halfDiagonalFov, Vector3.Angle(towardsPortalCenter, (corner2 - viewPos).normalized));
		// halfDiagonalFov = Mathf.Max(halfDiagonalFov, Vector3.Angle(towardsPortalCenter, (corner3 - viewPos).normalized));
		// halfDiagonalFov = Mathf.Max(halfDiagonalFov, Vector3.Angle(towardsPortalCenter, (corner4 - viewPos).normalized));
		// var diagonalFov = halfDiagonalFov * 2f;
		// convert to non-diagonal fov
		// var fov = Mathf.Atan(  Mathf.Tan(diagonalFov*Mathf.Deg2Rad/2f) / Mathf.Sqrt(2f))  * Mathf.Rad2Deg * 2f;

		// If we want calculation that is not dependent on the direction (of portalViewPointSource)
		// we can allow for a better range if calc is less accurate but we use more build in margin
		// (using diagonal fov)
		var s = portalSize * (1f + portalSizeMarginPercentage/100f);
		var corner1 = transform.TransformPoint(new Vector3(  s/2f,  s/2f, 0f));
		var corner2 = transform.TransformPoint(new Vector3( -s/2f, -s/2f, 0f));
		var corner3 = transform.TransformPoint(new Vector3(  s/2f, -s/2f, 0f));
		var corner4 = transform.TransformPoint(new Vector3( -s/2f,  s/2f, 0f));
		var diagonal1 = Vector3.Angle((corner1 - viewPos).normalized, (corner2 - viewPos).normalized);
		var diagonal2 = Vector3.Angle((corner3 - viewPos).normalized, (corner4 - viewPos).normalized);
		var fov = Mathf.Max(diagonal1, diagonal2);


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
