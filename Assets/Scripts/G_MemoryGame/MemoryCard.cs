using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MemoryCard : MonoBehaviour
{

    [SerializeField] private SpriteRenderer cardBack;

    [SerializeField] private Transform min;
    [SerializeField] private Transform max;



    private bool showing = false;
    private string spriteName = "";
    private bool paired = false;

    public bool Paired
    {
        get { return paired; }
        set { paired = value; }
    }

    public Bounds GetCardBounds()
    {
        return cardBack.bounds;
    }

    public string SpriteName
    {
        get { return spriteName; }
    }
    public bool Showing
    {
        get { return showing; }
    }

    public Transform Min { get => min; }
    public Transform Max { get => max; }

    public void InitCard()
    {
        spriteName = transform.Find("Front").GetComponent<SpriteRenderer>().sprite.name;
    }

    public void DisableCard()
    {
        GetComponent<BoxCollider2D>().enabled = false;
    }

    public void EnableCard()
    {
        GetComponent<BoxCollider2D>().enabled = true;
    }

    public void ShowCard(Action onShowedCard)
    {
        LeanTween.rotateY(gameObject, -180, 0.25f).setOnComplete(() =>
        {
            onShowedCard?.Invoke();
            showing = true;
        });
    }
    public void HideCard(Action onHiddenCard)
    {
        LeanTween.rotateY(gameObject, 0, 0.25f).setOnComplete(() =>
        {
            onHiddenCard?.Invoke();
            showing = false;
        });
    }
}
