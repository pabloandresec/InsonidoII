using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using System.Linq;
using UnityEngine.UI;

public class DiagDisplayerBubble : DiagDisplayer
{
    [SerializeField] protected TextMeshProUGUI[] namesTMP;
    [SerializeField] protected TextMeshProUGUI[] bubblesTMP;
    private bool scalingBubble = false;

    private void Start()
    {
        if (startOnStartLevel)
        {
            PrepareAndStart();
        }
    }

    protected override void PrepareAndStart()
    {
        foreach (RectTransform rt in positions)
        {
            rt.localScale = new Vector3(.75f, .75f, .75f);
            rt.GetComponent<Image>().color = new Color(.25f, .25f, .25f);
        }
        StartDialogCutscene(0);
    }

    protected override void StartDialogCutscene(int index)
    {
        if (cutsceneDialogs[index] == null)
        {
            Debug.LogError("Cutscene dialogpack non existant!");
            return;
        }

        StartCoroutine(TypeDialog(cutsceneDialogs[index]));
    }

    public void StartDialogPack(DialogPack pack)
    {
        if (pack == null)
        {
            Debug.LogError("Cutscene dialogpack non existant!");
            return;
        }

        StartCoroutine(TypeDialog(pack));
    }



    private void SetDialogUI(DialogPack dialogPack, int currentLine, TextMeshProUGUI bubble, TextMeshProUGUI name)
    {
        bubble.text = dialogPack.Dialog[currentLine].DialogText;
        name.text = dialogPack.Dialog[currentLine].CharacterName;
        foreach (RectTransform rt in positions)
        {
            string characterPos = dialogPack.Dialog[currentLine].CharacterPos.ToString();
            if (rt.transform.name == characterPos)
            {
                ShowCharacter(rt, () => {
                    switch (dialogPack.Dialog[currentLine].CharacterAnimation)
                    {
                        case CharacterAnimation.NONE:
                            break;
                        case CharacterAnimation.SHAKE:
                            LeanTween.moveLocalX(rt.gameObject, rt.localPosition.x + 10, .25f).setLoopPingPong(2);
                            break;
                    }
                });
            }
            if (rt.transform.name != characterPos)
            {
                HideCharacter(rt);
            }
        }
    }

    public IEnumerator TypeDialog(DialogPack dialogPack)
    {
        int currentLine = -1;
        bool dialogComplete = false;
        while (!dialogComplete)
        {
            currentLine++;

            if (currentLine >= dialogPack.Dialog.Length)
            {
                Debug.Log("Ended displaying dialog " + currentDialog);
                currentDialog++;
                dialogComplete = true;
                if (currentDialog >= cutsceneDialogs.Length)
                {
                    Debug.Log("Ended displaying full Cutscene");
                    onComplete?.Invoke();
                }
                else
                {
                    StartDialogCutscene(currentDialog);
                }
            }
            else
            {
                bool lineComplete = false;
                string positionName = dialogPack.Dialog[currentLine].CharacterPos.ToString();

                for (int i = 0; i < positions.Length; i++)
                {
                    positions[i].transform.GetChild(0).GetComponent<RectTransform>().localScale = Vector3.zero;
                    if (positions[i].transform.GetChild(0).name.Contains(positionName))
                    {
                        positions[i].transform.GetChild(0).gameObject.SetActive(true);
                        scalingBubble = true;
                        LeanTween.scale(positions[i].transform.GetChild(0).gameObject, Vector3.one, 0.25f).setOnComplete(() => {
                            scalingBubble = false;
                        });
                    }
                    else
                    {
                        positions[i].transform.GetChild(0).gameObject.SetActive(false);
                    }
                }

                while (scalingBubble) yield return null;

                TextMeshProUGUI name = namesTMP.FirstOrDefault( n => n.transform.name == positionName);
                TextMeshProUGUI text = bubblesTMP.FirstOrDefault(n => n.transform.name == positionName);

                SetDialogUI(dialogPack, currentLine, text , name);
                text.ForceMeshUpdate();
                int totalVisibleCharacters = text.textInfo.characterCount;
                int counter = 0;
                while (!lineComplete)
                {
                    int visibleCount = counter % (totalVisibleCharacters + 1);
                    text.maxVisibleCharacters = visibleCount;
                    if (visibleCount >= totalVisibleCharacters || skipped)
                    {

                        lineComplete = true;
                        text.maxVisibleCharacters = totalVisibleCharacters;
                        Debug.Log("Ended diplaying text " + currentLine);
                        if (skipped)
                        {
                            skipped = false;
                            while (!skipped)
                            {
                                yield return null;
                            }
                        }
                        else
                        {
                            while (!skipped)
                            {
                                yield return null;
                            }
                            skipped = false;
                        }
                    }

                    counter += 1;
                    yield return new WaitForSeconds(charTime);
                }
            }
        }
    }


    public override void SkipText()
    {
        base.SkipText();
        if (scalingBubble) return;
        skipped = true;
    }
}
