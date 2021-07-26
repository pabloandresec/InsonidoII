using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Dialog", menuName = "Data/Dialogs")]
[Serializable]
public class DialogPack : ScriptableObject
{
    [SerializeField] private string packName;
    [SerializeField] private Dialog[] dialog;

    public Dialog[] Dialog { get => dialog; }
    public string PackName { get => packName; }
}
