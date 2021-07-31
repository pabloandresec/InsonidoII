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

    private Coroutine spriteAnim;
    private int portraitTween;
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

            if(spriteAnim != null)
            {
                StopCoroutine(spriteAnim);
                spriteAnim = null;
            }
            spriteAnim = StartCoroutine(PlayCharacterAnimation(dialogPack.Dialog[currentLine].Sprites, dialogPack, currentLine));
            //character.sprite = dialogPack.Dialog[currentLine].Character;

            if (rt.transform.name == characterPos)
            {
                ShowCharacter(rt, () => {
                    LeanTween.cancel(rt.gameObject, portraitTween);
                    switch (dialogPack.Dialog[currentLine].CharacterAnimation)
                    {
                        case PotraitAnimation.NONE:
                            break;
                        case PotraitAnimation.SHAKE:
                            portraitTween = LeanTween.moveLocalX(rt.gameObject, rt.localPosition.x + 10, .25f).setLoopPingPong(2).id;
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

    private IEnumerator PlayCharacterAnimation(SpritePack sprPack, DialogPack dialogPack, int currentLine)
    {
        float tPassed = 0;
        float frameTime = 1f /(float)sprPack.Fps;
        SpritePackAnimationWrapMode spawm = dialogPack.Dialog[currentLine].SpriteAnimWrapMode;
        Debug.Log("FrameTime: " + frameTime + ", wrapMode: " + spawm);
        int currentFrame = 0;
        bool reverse = false;

        bool done = false;
        while (!done)
        {
            if(tPassed >= frameTime)
            {
                Debug.Log("Currentframe = " + currentFrame);
                currentFrame = !reverse ? currentFrame + 1 : currentFrame - 1; //next frame
                tPassed = 0; //reset timer
                if(currentFrame >= sprPack.Sprites.Length || currentFrame < 0) //if wrapping anim
                {
                    switch (spawm)
                    {
                        case SpritePackAnimationWrapMode.STOP:
                            Debug.Log("Animation ended!");
                            done = true;
                            break;
                        case SpritePackAnimationWrapMode.LOOP:
                            Debug.Log("Animation reset!");
                            currentFrame = 0;
                            break;
                        case SpritePackAnimationWrapMode.PINGPONG:
                            Debug.Log("Animation PingPong");
                            currentFrame = !reverse ? currentFrame - 2 : currentFrame + 2; //next frame
                            reverse = !reverse;
                            break;
                    }
                }
            }
            character.sprite = sprPack.Sprites[currentFrame];
            tPassed += Time.deltaTime;
            yield return null;
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
