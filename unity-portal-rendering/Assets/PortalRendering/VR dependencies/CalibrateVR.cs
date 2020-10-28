using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(UnityXRInputAdapter))]
public class CalibrateVR : MonoBehaviour
{
    public GameObject stage1;
    public GameObject stage2;
    public GameObject rightHandTransform;
    public GameObject portal;
    public GameObject outputCalibration;

    private int _calibrateStep = 1;
    private Vector3 _nearPoint;

    private bool _lastRTrigger = false; 
    void Update()
    {
        var inp = GetComponent<UnityXRInputAdapter>();
        if(inp.RightTrigger && !_lastRTrigger || Input.GetMouseButtonDown(0)) {
            _calibrateStep++;
            stage1.SetActive(false);
            stage2.SetActive(false);
            if(_calibrateStep == 1) {
                // get 
                outputCalibration.transform.position = Vector3.zero;
                outputCalibration.transform.rotation = Quaternion.identity;
                stage1.SetActive(true);
            } else if(_calibrateStep == 2) {
                stage2.SetActive(true);
                _nearPoint = rightHandTransform.transform.position;
            } else if(_calibrateStep == 3) {
                var farpoint = rightHandTransform.transform.position;
                var vrForward = _nearPoint - farpoint;
                vrForward.y = 0f;
                vrForward.Normalize();
                Matrix4x4 VRSpaceM = Matrix4x4.TRS(_nearPoint + vrForward*0.1f, Quaternion.LookRotation(vrForward, Vector3.up), Vector3.one);
                var portalM = portal.transform.localToWorldMatrix;
                var calM = portalM * VRSpaceM.inverse;
                outputCalibration.transform.position = calM.GetColumn(3);
                outputCalibration.transform.rotation = calM.rotation;
                _calibrateStep = 0;
            }
        }
        _lastRTrigger = inp.RightTrigger;
    }
}
