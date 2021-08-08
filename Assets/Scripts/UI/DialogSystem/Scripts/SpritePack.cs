using System;
using UnityEngine;

[CreateAssetMenu(fileName = "SpritePack", menuName = "Data/SpritePack")]
public class SpritePack : ScriptableObject
{
    [SerializeField] private string packname;
    [Min(1)]
    [SerializeField] private int fps;
    [SerializeField] private Sprite[] sprites;

    public string Packname { get => packname;}
    public int Fps { get => fps; }
    public Sprite[] Sprites { get => sprites; }
}