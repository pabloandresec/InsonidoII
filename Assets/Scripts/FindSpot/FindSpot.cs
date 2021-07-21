using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using Random = UnityEngine.Random;
using UnityEngine.SceneManagement;

public class FindSpot : Puzzle
{
#pragma warning disable 0649

    [SerializeField] private Transform itemsParent;
    [SerializeField] private Transform boxesParent;
    [SerializeField] private Dictionary<GameObject, GameObject> pairs;

    public static bool android = false;

#pragma warning restore 0649


    public override void StartPuzzle()
    {
        base.StartPuzzle();
        android = false;

#if UNITY_ANDROID
        android = true;
#endif
        pairs = new Dictionary<GameObject, GameObject>();
        List<GameObject> items = GameObject.FindGameObjectsWithTag("Item").ToList();

        for (int i = 0; i < boxesParent.childCount; i++)
        {
            GameObject selectedItem = items[Random.Range(0, items.Count)];
            pairs.Add(boxesParent.GetChild(i).gameObject, selectedItem);
            items.Remove(selectedItem);
        }

        foreach(KeyValuePair<GameObject,GameObject> p in pairs)
        {
            p.Value.GetComponent<Item>().SetTGT(p.Key.transform);
        }
    }

    public void CheckForCompletition()
    {
        if(itemsParent.childCount == 0)
        {
            Debug.Log("Game Completed!");
            SceneManager.LoadScene(0);
        }
    }
}
