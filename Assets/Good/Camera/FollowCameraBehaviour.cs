using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCameraBehaviour : MonoBehaviour
{
    public GameObject pov;
    public new Camera camera;
    public GameObject cursor1;
    public GameObject cursor2;
    public float smoothSmartPovPerSec = 0.5f;
    private Vector3 smartPovPosition;
    private Vector3 smoothSmartPovPosition;
    private Vector3 currentPosition;
    private Vector3 previousPosition;
    private Vector3 velocity;

    // stupid code TODO: need to rewrite from scratch

    void Start() {
        currentPosition = pov.transform.position;
        smartPovPosition = currentPosition;
        smoothSmartPovPosition = currentPosition;
        previousPosition = currentPosition;
        velocity = Vector3.zero;
    }
    void CalcSmartTrackParameters__FixedUpdate() {
        previousPosition = currentPosition;
        currentPosition = pov.transform.position;
        velocity = (currentPosition - previousPosition) / Time.fixedDeltaTime; 
        smartPovPosition = currentPosition + velocity * 1f;
        float smoothSmartPovPerFrame = Mathf.Pow(smoothSmartPovPerSec, Time.fixedDeltaTime);
        smoothSmartPovPosition = smartPovPosition + (smoothSmartPovPosition - smartPovPosition) * smoothSmartPovPerFrame;
        smoothSmartPovPosition.y = Mathf.Max(0, smoothSmartPovPosition.y);
    }
    void FixedUpdate()
    {
        CalcSmartTrackParameters__FixedUpdate();
    }
    void Update()
    {
        float velocityFovRatio = Mathf.Min(1, velocity.magnitude / 100);
        float heightFovRatio = Mathf.Min(1, currentPosition.y / 100);
        float fov = 25 + Mathf.Pow(velocityFovRatio, 2) * 5 + heightFovRatio * 10;
        camera.fieldOfView = fov;
        Vector3 shiftedPovPosition = smoothSmartPovPosition;
        float baselineY = 30;
        float heightRatio = (shiftedPovPosition.y + 2f) / baselineY;
        float nonlinearHeightRatio = Mathf.Pow(heightRatio, 1/2f);
        float nonlinearHeight = nonlinearHeightRatio * baselineY - 2f;
        shiftedPovPosition.y = nonlinearHeight * 0.75f; 
        Vector3 cameraPosition = camera.transform.position;
        cameraPosition.x = smoothSmartPovPosition.x;
        cameraPosition.y = baselineY * 0.75f - smoothSmartPovPosition.y / 7.5f;
        camera.transform.position = cameraPosition;
        camera.transform.LookAt(shiftedPovPosition);
        if (cursor1) {
            cursor1.transform.position = smoothSmartPovPosition;
        }
        if (cursor2) {
            cursor2.transform.position = shiftedPovPosition;
        }
        
        
        // var a = camera.transform.rotation.eulerAngles;
        // a.z = pov.transform.rotation.eulerAngles.z;
        // Quaternion rotation = Quaternion.Euler(a);

        // camera.transform.rotation = rotation;
    }


}
