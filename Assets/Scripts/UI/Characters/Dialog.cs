using System;
using UnityEngine;

[Serializable]
public class Dialog
{
    [SerializeField] private string characterName;
    [SerializeField] private Sprite character;
    [SerializeField] private Positions characterPos = Positions.P1;
    [SerializeField] private CharacterAnimation characterAnimation = CharacterAnimation.NONE;
    [TextArea()]
    [SerializeField] private string dialog;
    [SerializeField] private AudioClip clip;

    public string CharacterName { get => characterName; }
    public Sprite Character { get => character; }
    public string DialogText { get => dialog; }
    public AudioClip Clip { get => clip; }
    public CharacterAnimation CharacterAnimation { get => characterAnimation; }
    public Positions CharacterPos { get => characterPos; }
}
public enum Positions
{
    P1,
    P2
}
public enum CharacterAnimation
{
    NONE,
    SHAKE
}