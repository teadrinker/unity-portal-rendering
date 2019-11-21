using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class UpdatePortalRendering : MonoBehaviour {

	public float portalSize = 0.4f;

	[Range(0f, 100f)]
	public float portalSizeMarginPercentage = 30f; // need margin unless we only see from the front

	[Space()]

//	public GameObject portalTransform;
	public GameObject portalViewPointSource;
	public GameObject portalQuad;
	public Material realtimeProjectionMat;
	public Camera portalRenderCam;
	public Camera exportCam;


	//Matrix4x4 fromPortalSpace() {
		//var matDest = portalTransform.transform.localToWorldMatrix;
		//var matPort = transform.localToWorldMatrix;

		//return (matPort.inverse * matDest).inverse;
	//}
	Vector3 toPortalSpace(Vector3 pos) {
		return pos;		
		//var matDest = portalTransform.transform.localToWorldMatrix;
		//var matPort = transform.localToWorldMatrix;

		//return (matPort.inverse * matDest).MultiplyPoint(pos);
	}

	void Update() {

		portalRenderCam.transform.position = toPortalSpace(portalViewPointSource.transform.position);
		portalRenderCam.transform.LookAt( toPortalSpace(portalQuad.transform.position) );

		var portalDir = Vector3.back;

		var distanceToPortal = (portalViewPointSource.transform.position - portalQuad.transform.position).magnitude;
		
		var radius = (portalSize * (1f + portalSizeMarginPercentage/100f)) / 2f;

		// relationship between fov and camera distance ( *2f because radius is half fov)
		var fov = Mathf.Atan2(radius, distanceToPortal) * Mathf.Rad2Deg * 2f;

		portalRenderCam.fieldOfView = fov;
		exportCam.orthographicSize = portalSize * 0.5f;
		portalQuad.transform.localScale = Vector3.one * portalSize;

		RealtimeUVProjectionTexture(portalRenderCam, realtimeProjectionMat);
	}


	void RealtimeUVProjectionTexture(Camera MatrixSource, Material DestinationMaterial) {
		if(MatrixSource != null && DestinationMaterial != null) {
			var P = MatrixSource.projectionMatrix;
			//P = GL.GetGPUProjectionMatrix(P, true);
			Matrix4x4 V = MatrixSource.worldToCameraMatrix;
			//V = fromPortalSpace() * V; // this is not correct...

			Matrix4x4 VP = P * V;
			DestinationMaterial.SetMatrix("_UVProjMatrix", VP);
		}
	}

}
