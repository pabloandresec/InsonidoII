using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using Cinemachine;
using Random = UnityEngine.Random;

public class FindSpotSpriteEd : Puzzle
{
#pragma warning disable 0649
    [SerializeField] private Vector2Int gridSize;
    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private GameObject itemPrefab;
    [SerializeField] private VibrationWave wave;
    [SerializeField] private List<Sprite> sprites;
    [SerializeField] private LayerMask slotMask;
    [SerializeField] private Transform itemsParent;
    [SerializeField] private Transform slotsParent;
    [SerializeField] private float distanceSensitivity = 2f;
    [Header("Cam")]
    [SerializeField] private CinemachineVirtualCamera cam;
    [SerializeField] private float borderSize = 1f;
    [SerializeField] private Transform[] additionalBoundPoints;

    private CinemachineBasicMultiChannelPerlin camNoise;
    private Dictionary<string, string> pairs;
    private Transform draggingObject = null;
    private Transform tgtSlot = null;
    private Vector3 ogPos = Vector3.zero;
#pragma warning restore 0649

    public override void StartPuzzle()
    {
        base.StartPuzzle();

        pairs = new Dictionary<string, string>();
        List<Sprite> mixedItems = sprites;
        mixedItems.Shuffle();
        List<GameObject> slots = new List<GameObject>();
        List<GameObject> items = new List<GameObject>();
        Bounds bounds = new Bounds();
        Vector3 offsetPos = new Vector3(gridSize.x + 1, 0, 0);
        int itemIndex = 0;
        for (int y = 0; y < gridSize.y; y++)
        {
            for (int x = 0; x < gridSize.x; x++)
            {
                GameObject slot = Instantiate(slotPrefab, new Vector3(x, y, 0), Quaternion.identity);
                slot.transform.SetParent(slotsParent);
                slot.transform.name = "X:" + x + ", Y:" + y;
                slots.Add(slot);
                bounds.Encapsulate(slot.transform.position);
                GameObject item = Instantiate(itemPrefab, new Vector3(x, y, 0) + offsetPos, Quaternion.identity);
                bounds.Encapsulate(item.transform.position);
                item.GetComponent<SpriteRenderer>().sprite = sprites[itemIndex];
                item.transform.name = itemIndex + "_" +sprites[itemIndex].name;
                item.transform.SetParent(itemsParent);
                items.Add(item);
                itemIndex++;
            }
        }

        int ind = 0;
        while(slots.Count != 0)
        {
            int randSlotIndex = Random.Range(0, slots.Count);
            pairs.Add(slots[randSlotIndex].transform.name, items[ind].transform.name);
            slots.RemoveAt(randSlotIndex);
            ind++;
        }

        Debug.Log(pairs.Count + " slot assigned!");

        if(additionalBoundPoints != null && additionalBoundPoints.Length > 0)
        {
            for (int i = 0; i < additionalBoundPoints.Length; i++)
            {
                bounds.Encapsulate(additionalBoundPoints[i].position);
            }
        }

        Utils.SetVirtualCameraInMiddleOfBounds(bounds, cam, borderSize);
        camNoise = cam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        camNoise.m_AmplitudeGain = 0;
        //Camera.main.transform.position -= new Vector3(0, borderSize / 2, 0);
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Vector3 pointerPos = GetMouse2DPos();
            Collider2D col = Physics2D.OverlapCircle(pointerPos, 0.05f);

            if(col != null)
            {
                if(col.CompareTag("Item"))
                {
                    wave.SetVibrationAmount(0);
                    wave.SetVibration(true);
                    tgtSlot = GetTGT(col.transform.name);
                    draggingObject = col.transform;
                    ogPos = draggingObject.position;
                }
            }
        }
        if(draggingObject != null)
        {
            Vector3 pointerPos = GetMouse2DPos();
            draggingObject.transform.position = pointerPos;

            if(tgtSlot != null)
            {
                float dist = Vector2.Distance(draggingObject.position, tgtSlot.position);
                float vibrationAmount = Mathf.InverseLerp(distanceSensitivity, 0, dist);
                wave.SetVibrationAmount(vibrationAmount);
                camNoise.m_AmplitudeGain = vibrationAmount;
            }
        }

        if (Input.GetMouseButtonUp(0) && draggingObject != null)
        {
            Vector3 pointerPos = GetMouse2DPos();
            Collider2D col = Physics2D.OverlapCircle(pointerPos, 0.05f, slotMask);
            if (col != null)
            {
                Debug.Log("Comparing " + col.transform.name + " with " + draggingObject.name);
                if(pairs[col.transform.name] == draggingObject.name)
                {
                    draggingObject.SetParent(col.transform);
                    draggingObject.localPosition = Vector3.zero;
                }
                else
                {
                    draggingObject.transform.position = ogPos;
                }
            }
            else
            {
                draggingObject.transform.position = ogPos;
            }
            wave.SetVibrationAmount(0);
            wave.SetVibration(false);
            camNoise.m_AmplitudeGain = 0;
            draggingObject = null;
        }
    }

    private Transform GetTGT(string name)
    {
        foreach(KeyValuePair<string,string> kvp in pairs)
        {
            if(kvp.Value == name)
            {
                return slotsParent.Find(kvp.Key);
            }
        }

        return null;
    }

    private static Vector3 GetMouse2DPos()
    {
        Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        pos.z = 0;
        return pos;
    }
}
