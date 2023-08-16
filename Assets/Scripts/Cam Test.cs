using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

public class CamTest : MonoBehaviour
{

    public Vector2Int debugPointCount = new Vector2Int(16, 9);
    [Range(0.1f, 10.0f)]
    public float debugPointRadius;
    public Color pointColor = Color.white;
    private bool cameraParametersChanged = true; // Flag to indicate camera parameter changes
    private List<GameObject> debugPoints = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        // Set the camera's clear flag to Solid Color
        debugPointRadius = 1.63f;
        Camera.main.clearFlags = CameraClearFlags.SolidColor;

        // Set the background color to whitish blue
        Camera.main.backgroundColor = new Color(0.3f, 0.3f, 0.9f);
        CameraRayTest();
    }

    private void OnValidate()
    {
        cameraParametersChanged = true; // Mark that camera parameters have changed
    }

    void DrawPoint(Vector3 position, Transform parent)
    {
        GameObject newPoint = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        newPoint.transform.position = position;
        newPoint.transform.localScale = new Vector3(0.01f * debugPointRadius, 0.01f * debugPointRadius, 0.01f * debugPointRadius); // Adjust the scale as needed
        newPoint.GetComponent<Renderer>().material.color = pointColor; // Adjust the color as needed

        newPoint.transform.parent = parent; // Attach the sphere to the camera

        debugPoints.Add(newPoint);
    }

    void CameraRayTest()
    {

        foreach (GameObject point in debugPoints)
        {
            Destroy(point); // Destroy existing debug points
        }
        debugPoints.Clear(); // Clear the list

        Camera cam = Camera.main;
        Transform camT = cam.transform;

        // Calculate width and height of projection plane.
        float planeHeight = cam.nearClipPlane * Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad) * 2;
        float planeWidth = planeHeight * cam.aspect;

        // Bottom left corner of the plane (in camera's local space)
        Vector3 bottomLeftLocal = new Vector3(-planeWidth / 2, -planeHeight / 2, cam.nearClipPlane);

        // Draw a grid of points on the plane
        for (int x = 0; x < debugPointCount.x; x++)
        {
            for (int y = 0; y < debugPointCount.y; y++)
            {
                float tx = x / (debugPointCount.x - 1f); // 0 = left edge of plane, 1 = right edge
                float ty = y / (debugPointCount.y - 1f); // 0 = bottom edge of plane, 1 = top edge


                // Calculate point in camera's local space, and then transform to world space
                Vector3 pointLocal = bottomLeftLocal + new Vector3(planeWidth * tx, planeHeight * ty, 0.01f);
                Vector3 point = camT.position + camT.right * pointLocal.x + camT.up * pointLocal.y + camT.forward * pointLocal.z;

                //Visualize
                DrawPoint(point, camT);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (cameraParametersChanged)
        {
            CameraRayTest();
            cameraParametersChanged = false; // Reset the flag
        }
    }
}


