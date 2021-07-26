using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VibrationWave : MonoBehaviour
{

#pragma warning disable 0649

    [SerializeField] private LineRenderer wave;
    [SerializeField] private bool vibrate;
    private float tPassed = 0;
    [Range(0f,1f)]
    [SerializeField] private float vibrationAmount = 0.1f;
    [Range(0.001f, 5f)]
    [SerializeField] private float vibrationScale = 2f;
    [SerializeField] private float vibrationSpeed = 2f;
    [Header("Setup")]
    [Range(1f, 500)]
    [SerializeField]
    private int waveVertexAmount = 300;

    [SerializeField] Transform worldStartTransform;
    [SerializeField] Transform worldEndTransform;

    private Vector3 worldStartPos;
    private Vector3 worldEndPos;

#pragma warning restore 0649


    private void Start()
    {
        SetupWave();
    }

    private void SetupWave()
    {
        worldStartPos = worldStartTransform.position;
        worldEndPos = worldEndTransform.position;

        GameObject start = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        start.transform.localScale = new Vector3(.1f, .1f, .1f);
        worldStartPos.z = 1;
        start.transform.position = worldStartPos;
        start.SetActive(false);

        GameObject end = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        end.transform.localScale = new Vector3(.1f, .1f, .1f);
        worldEndPos.z = 1;
        end.transform.position = worldEndPos;
        end.SetActive(false);

        float totalDistance = Vector3.Distance(start.transform.position, end.transform.position);
        float stepSize = totalDistance / waveVertexAmount;

        Vector3 currentLoopPos = start.transform.position;
        Vector3 dir = end.transform.position - start.transform.position;
        dir.Normalize();
        wave.positionCount = waveVertexAmount;
        for (int point = 0; point < waveVertexAmount; point++)
        {
            wave.SetPosition(point, currentLoopPos);
            Vector3 displacement = dir * stepSize;
            currentLoopPos += displacement;
        }

        wave.startWidth = stepSize;
        wave.endWidth = stepSize;
    }

    private void Update()
    {
        if (vibrate)
        {
            tPassed += Time.deltaTime * vibrationSpeed;
            for (int i = 0; i < wave.positionCount; i++)
            {
                Vector3 pointPosition = wave.GetPosition(i);
                float offset = Mathf.Sin(i + tPassed) * vibrationAmount * vibrationScale;
                pointPosition = new Vector3(pointPosition.x, worldStartPos.y + offset , 1);
                wave.SetPosition(i, pointPosition);
            }
        }
    }

    public void SetVibration(bool state)
    {
        vibrate = state;
        SetVibrationAmount(0);
        FlatLineWave();
        //Debug.Log("Vibration set to " + state);
    }

    private void FlatLineWave()
    {
        for (int i = 0; i < wave.positionCount; i++)
        {
            Vector3 pointPosition = wave.GetPosition(i);
            pointPosition = new Vector3(pointPosition.x, worldStartPos.y, 1);
            wave.SetPosition(i, pointPosition);
        }
        Color c = Color.green;
        wave.startColor = c;
        wave.endColor = c;
    }

    /// <summary>
    /// sets the vibration amount (value amount between 0f-1f)
    /// </summary>
    /// <param name="amount"></param>
    public void SetVibrationAmount(float amount)
    {
        float val = Mathf.Clamp01(amount);
        Color c = Color.Lerp(Color.green, Color.red, val);
        wave.startColor = c;
        wave.endColor = c;
        vibrationAmount = val;
        vibrationSpeed = Mathf.Lerp(0, 60, val);
    }


}
