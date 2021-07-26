using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CableFiller))]
public class CableFillerDrawer : Editor
{
    private CableFiller tgt;

    private void OnEnable()
    {
        tgt = (CableFiller)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if(GUILayout.Button("REGEN"))
        {
            tgt.ReGenerateGrid();
        }
    }
}