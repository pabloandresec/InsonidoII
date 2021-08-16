using UnityEngine;
using System.Collections;
using System;

public class PathPiece : MonoBehaviour
{
#pragma warning disable 0649

    [SerializeField] private Transform[] testPoints;

    [SerializeField] private bool canRotate = true;

    private bool rotating = false;
    private float[] rotations = new float[] {0,90,180,270 };
    private PolygonCollider2D pCol;
    private CircleCollider2D cCol;

#pragma warning restore 0649


    private void Awake()
    {
        pCol = GetComponent<PolygonCollider2D>();
        cCol = GetComponent<CircleCollider2D>();
    }

    public void ToggleColliderForPathfinding()
    {
        pCol.enabled = true;
        cCol.enabled = false;
    }

    public void ToggleColliderForRotation()
    {
        pCol.enabled = false;
        cCol.enabled = true;
    }

    public bool TestPoints()
    {
        int accuracy = 0;
        for (int i = 0; i < testPoints.Length; i++)
        {
            if(Physics2D.OverlapCircle(testPoints[i].position, 0.05f))
            {
                accuracy += 1;
            }
            else
            {
                accuracy -= 1;
            }
        }

        //Debug.Log(transform.name + " contacts at rot(" + transform.eulerAngles.z + ") are " + accuracy);

        if(accuracy == testPoints.Length)
        {
            return true;
        }
        return false;
    }

    public void RotatePiece(Action OnRotationEnd)
    {
        if(rotating || !canRotate)
        {
            return;
        }
        rotating = true;
        int nextIndex = CheckCurrentRotationIndex();
        nextIndex++;
        if(nextIndex >= rotations.Length)
        {
            nextIndex = 0;
        }
        LeanTween.rotateZ(gameObject, rotations[nextIndex], 0.2f).setOnComplete(() => {
            rotating = false;
            OnRotationEnd?.Invoke();
        });
    }

    private int CheckCurrentRotationIndex()
    {
        for (int i = 0; i < rotations.Length; i++)
        {
            float r = rotations[i];
            if (r == transform.eulerAngles.z)
            {
                return i;
            }
        }
        return 0;
    }
}
