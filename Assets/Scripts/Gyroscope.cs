using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gyroscope : Puzzle
{
#pragma warning disable 0649

    [SerializeField] private GameObject tgt;
    [SerializeField] private float maxSpeed = 0.1f;
    private Rigidbody tgtRB;

#pragma warning restore 0649


    public override void StartPuzzle()
    {
        base.StartPuzzle();
        tgtRB = tgt.GetComponent<Rigidbody>();
        /*
        if (SystemInfo.supportsGyroscope)
        {
            Input.gyro.enabled = true;
            Puzzle.SetMobileDebugState(true);
        }
        */
    }

    private void FixedUpdate()
    {
        GyroModifyCamera();
    }

    public static float ConvertRangedValueToAnotherRange(float value, FloatRange oldRange, FloatRange newRange)
    {
        float oR = (oldRange.max - oldRange.min);
        float nR = (newRange.max - newRange.min);
        return (((value - oldRange.min) * nR) / oR) + newRange.min;
    }

    private void GyroModifyCamera()
    {
        //Quaternion phoneAttitude = GyroToUnity(Input.gyro.attitude);

        Vector3 eulerRot = Input.acceleration;
        float x = eulerRot.x;
        eulerRot.x = eulerRot.y;
        eulerRot.z = -x;
        eulerRot.y = 0;
            /*
        eulerRot += new Vector3(90, 0, 90);

        Vector3 testValues = new Vector3(
            ConvertRangedValueToAnotherRange(eulerRot.x, new FloatRange(70,100), new FloatRange(-1,1)),
            0,
            ConvertRangedValueToAnotherRange(eulerRot.z, new FloatRange(70, 100), new FloatRange(-1, 1))
            );

        //eulerRot = new Vector3(Mathf.InverseLerp(70, 110, eulerRot.x), 0, Mathf.InverseLerp(70, 110, eulerRot.z));
        */
        tgtRB.AddTorque(eulerRot);
        tgtRB.velocity = Vector3.ClampMagnitude(tgtRB.velocity, maxSpeed);
        Puzzle.MobileDebug(eulerRot.ToString());
        //tgt.transform.rotation = phoneAttitude;
    }

    private static Quaternion GyroToUnity(Quaternion q)
    {
        return new Quaternion(q.x, q.y, -q.z, -q.w);
    }
}
[Serializable]
public struct FloatRange
{
    public float min;
    public float max;

    public FloatRange(float min, float max)
    {
        this.min = min;
        this.max = max;
    }

    public FloatRange GetPercentRange()
    {
        return new FloatRange(0, 100);
    }

}