using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using System.Linq;
using UnityEngine.UI;

public class DiagDisplayerVN : DiagDisplayer
{
    [SerializeField] protected TextMeshProUGUI nameTMP;
    [SerializeField] protected TextMeshProUGUI textboxTMP;
    [SerializeField] protected Image character;

    private AudioController audioController;

    private void Start()
    {
        audioController = GameObject.FindGameObjectWithTag("AudioController").GetComponent<AudioController>();
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
        if(cutsceneDialogs[index] == null)
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

    

    private void SetDialogUI(DialogPack dialogPack, int currentLine)
    {
        textboxTMP.text = dialogPack.Dialog[currentLine].DialogText;
        nameTMP.text = dialogPack.Dialog[currentLine].CharacterName;
        foreach (RectTransform rt in positions)
        {
            string characterPos = dialogPack.Dialog[currentLine].CharacterPos.ToString();
            Debug.Log(characterPos + " Position selected");

            character.sprite = dialogPack.Dialog[currentLine].Character;

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
        if(dialogPack.Dialog[currentLine].Clip != null)
        {
            audioController.PlaySFX(dialogPack.Dialog[currentLine].Clip);
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
                SetDialogUI(dialogPack, currentLine);
                textboxTMP.ForceMeshUpdate();
                int totalVisibleCharacters = textboxTMP.textInfo.characterCount;
                int counter = 0;
                while (!lineComplete)
                {
                    int visibleCount = counter % (totalVisibleCharacters + 1);
                    textboxTMP.maxVisibleCharacters = visibleCount;
                    if (visibleCount >= totalVisibleCharacters || skipped)
                    {

                        lineComplete = true;
                        textboxTMP.maxVisibleCharacters = totalVisibleCharacters;
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
        skipped = true;
    }
}
