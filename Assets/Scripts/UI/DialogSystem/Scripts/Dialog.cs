using System;
using UnityEngine;

[Serializable]
public class Dialog
{
    [SerializeField] private string characterName;
    [SerializeField] private SpritePack sprites;
    [SerializeField] private SpritePackAnimationWrapMode spriteAnimWrapMode = SpritePackAnimationWrapMode.STOP;
    [SerializeField] private Positions characterPos = Positions.P1;
    [SerializeField] private PotraitAnimation portraitAnim = PotraitAnimation.NONE;
    [TextArea()]
    [SerializeField] private string dialog;
    [SerializeField] private AudioClip clip;

    public string CharacterName { get => characterName; }
    public SpritePack Sprites { get => sprites; }
    public string DialogText { get => dialog; }
    public AudioClip Clip { get => clip; }
    public PotraitAnimation CharacterAnimation { get => portraitAnim; }
    public Positions CharacterPos { get => characterPos; }
    public SpritePackAnimationWrapMode SpriteAnimWrapMode { get => spriteAnimWrapMode; }
}
public enum Positions
{
    P1,
    P2
}

public enum SpritePackAnimationWrapMode
{
    STOP,
    LOOP,
    PINGPONG
}
public enum PotraitAnimation
{
    NONE,
    SHAKE
}