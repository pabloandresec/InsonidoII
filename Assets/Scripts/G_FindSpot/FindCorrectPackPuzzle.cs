using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Cinemachine;
using UnityEngine.SceneManagement;

public class FindCorrectPackPuzzle : Puzzle
{
    [SerializeField] private Vector2Int gridSize;
    [SerializeField] private Vector2Int gridStartOffset;
    [SerializeField] private PackItemsRelationship[] relationships;
    [Header("settings")]
    [SerializeField] private LayerMask slotMask;
    [SerializeField] private float distanceSensitivity = 2f;
    [SerializeField] private float borderSize = 1f;
    [SerializeField] private Vector3 boardOffset;
    [Header("prefabs")]
    [SerializeField] private GameObject itemPrefab;
    [Header("references")]
    [SerializeField] private Transform packParent;
    [SerializeField] private Transform itemsParent;
    [SerializeField] private CinemachineVirtualCamera cam;
    [SerializeField] private SpriteRenderer board;
    [Header("Sounds")]
    [SerializeField] private AudioClip onGrab;
    [SerializeField] private AudioClip onRelease;
    [SerializeField] private AudioClip onCorrectItemDrop;
    [SerializeField] private AudioClip onWrongItemDrop;
    [SerializeField] private AudioClip onGameCompleted;

    private AudioController ac;
    private CinemachineBasicMultiChannelPerlin camNoise;
    private Dictionary<Transform, Transform> pairs;
    private Transform draggingObject = null;
    private Transform tgtSlot = null;
    private Vector3 ogPos = Vector3.zero;
    private Bounds bounds;

    public override void StartPuzzle()
    {
        base.StartPuzzle();
        

        SetupPacks();
        SetBounds();
        SetupCamera();
        ResizeSpriteToScreen(cam, spriteRenderer);
        ResizeSpriteToScreen(cam, tutorial);
        SetBorders();

        ac = GameObject.FindGameObjectWithTag("AudioController").GetComponent<AudioController>();
    }

    private void SetBorders()
    {
        board.transform.position = bounds.center;
        board.size = bounds.size + boardOffset;
    }

    private void SetBounds()
    {
        for (int i = 0; i < itemsParent.childCount; i++)
        {
            bounds.Encapsulate(itemsParent.GetChild(i).GetChild(0).position);
            bounds.Encapsulate(itemsParent.GetChild(i).GetChild(1).position);
        }
        for (int i = 0; i < packParent.childCount; i++)
        {
            bounds.Encapsulate(packParent.GetChild(i).GetChild(0).position);
            bounds.Encapsulate(packParent.GetChild(i).GetChild(1).position);
        }
    }

    private void SetupCamera()
    {
        Utils.SetVirtualCameraInMiddleOfBounds(bounds, cam, borderSize);
        camNoise = cam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        camNoise.m_AmplitudeGain = 0;
    }

    private void SetupPacks()
    {
        int currentY = 0;
        List<GameObject> items = new List<GameObject>();
        pairs = new Dictionary<Transform, Transform>();

        for (int i = 0; i < relationships.Length; i++)
        {
            Transform currentPack = SpawnPack(currentY, i);
            currentY++;

            for (int j = 0; j < relationships[i].Items.Length; j++)
            {
                Transform currentItem = SpawnItem(i, j);
                items.Add(currentItem.gameObject);
                pairs.Add(currentItem, currentPack);
            }
        }

        items.Shuffle();

        int currentItemIndex = 0;
        for (int y = gridStartOffset.y; y < gridSize.y + gridStartOffset.y; y++)
        {
            for (int x = gridStartOffset.x; x < gridSize.x + gridStartOffset.x; x++)
            {
                if(currentItemIndex >= items.Count)
                {
                    break;
                }
                items[currentItemIndex].transform.position = new Vector3(x, y, 0);
                items[currentItemIndex].transform.SetParent(itemsParent);
                items[currentItemIndex].transform.name = items[currentItemIndex].GetComponent<SpriteRenderer>().sprite.name;
                currentItemIndex++;
            }
        }

    }

    private Transform SpawnItem(int relationshipIndex, int itemIndex)
    {
        GameObject item = Instantiate(itemPrefab, Vector3.zero, Quaternion.identity);
        SpriteRenderer spriteRenderer = item.GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = relationships[relationshipIndex].Items[itemIndex];
        spriteRenderer.sortingOrder++;
        return item.transform;
    }

    private Transform SpawnPack(int currentY, int i)
    {
        GameObject pack = Instantiate(itemPrefab, new Vector3(0, currentY, 0), Quaternion.identity);
        pack.layer = 8;
        pack.tag = "Untagged";
        pack.GetComponent<SpriteRenderer>().sprite = relationships[i].Pack;
        pack.transform.SetParent(packParent);
        pack.transform.name = relationships[i].Pack.name;
        return pack.transform;
    }

    private void Update()
    {
        if(paused)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            Vector3 pointerPos = GetMouse2DPos();
            Collider2D col = Physics2D.OverlapCircle(pointerPos, 0.05f);

            if (col != null)
            {
                if (col.CompareTag("Item"))
                {
                    tgtSlot = GetTGT(col.transform);
                    draggingObject = col.transform;
                    ogPos = draggingObject.position;
                    ac.PlaySFX(onGrab);
                }
            }
        }
        if (draggingObject != null)
        {
            Vector3 pointerPos = GetMouse2DPos();
            draggingObject.position = pointerPos;

            if (tgtSlot != null)
            {
                float dist = Vector2.Distance(draggingObject.position, tgtSlot.position);
                float vibrationAmount = Mathf.InverseLerp(distanceSensitivity, 0, dist);
                camNoise.m_AmplitudeGain = vibrationAmount;
            }
        }

        if (Input.GetMouseButtonUp(0) && draggingObject != null)
        {
            ac.PlaySFX(onRelease);
            Vector3 pointerPos = GetMouse2DPos();
            Collider2D col = Physics2D.OverlapCircle(pointerPos, 0.05f, slotMask);
            if (col != null)
            {
                //Debug.Log("Comparing " + col.transform.name + " with " + draggingObject.name);
                if (pairs[draggingObject] == col.transform)
                {
                    draggingObject.SetParent(col.transform);
                    draggingObject.localPosition = Vector3.zero;
                    draggingObject.gameObject.SetActive(false);
                    ac.PlaySFX(onCorrectItemDrop);
                    CheckGameCompletition();
                }
                else
                {
                    ac.PlaySFX(onWrongItemDrop);
                    draggingObject.transform.position = ogPos;
                }
            }
            else
            {
                ac.PlaySFX(onWrongItemDrop);
                draggingObject.transform.position = ogPos;
            }
            camNoise.m_AmplitudeGain = 0;
            draggingObject = null;
        }
    }

    private void CheckGameCompletition()
    {
        if(itemsParent.childCount == 0)
        {
            ac.PlaySFX(onGameCompleted);
            Debug.Log("GAME COMPLETED!");
            SceneManager.LoadScene(0);
        }
    }

    private Transform GetTGT(Transform key)
    {
        foreach (KeyValuePair<Transform, Transform> kvp in pairs)
        {
            if (kvp.Key == key)
            {
                return kvp.Value;
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

    public override void PauseGame()
    {
        Debug.Log("PAUSING GAME");
        GameObject.FindGameObjectWithTag("UI").GetComponent<MenuController>().SwapMenu(1);
        paused = true;
    }

    public override void ResumeGame()
    {
        Debug.Log("RESUMING GAME");
        GameObject.FindGameObjectWithTag("UI").GetComponent<MenuController>().SwapMenu(0);
        paused = false;
    }

    public override void RestartGame()
    {
        Debug.Log("RESETTING GAME");
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
    }

    public override void ShowTutorial(bool show)
    {
        Debug.Log("SHOWING TUTORIAL -> " + show);
        paused = show;
        tutorial.gameObject.SetActive(show);
    }
}

[Serializable]
public struct PackItemsRelationship
{
    [SerializeField] private Sprite pack;
    [SerializeField] private Sprite[] items;

    public Sprite Pack { get => pack; }
    public Sprite[] Items { get => items; }
}