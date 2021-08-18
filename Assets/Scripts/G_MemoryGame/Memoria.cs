using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class Memoria : Puzzle
{
#pragma warning disable 0649

    [SerializeField] private Vector2Int gridSize;
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private Sprite[] sprites;
    [SerializeField] private LayerMask cardsMask;
    [SerializeField] private MemoryCard lastCard = null;
    [Header("Cam")]
    [SerializeField] private float border = 1f;
    [SerializeField] private Vector3 boardScaleOffset; 
    [SerializeField] private SpriteRenderer board;
    [Header("Sound")]
    [SerializeField] private AudioClip onFlip;
    [SerializeField] private AudioClip onCorrectGuess;
    [SerializeField] private AudioClip onWrongGuess;
    [SerializeField] private AudioClip onGameCompleted;

    private AudioController ac;
    private bool blockClicks = false;
    private List<GameObject> spawnedCards = null;
    Bounds boardBounds;

#pragma warning restore 0649


    public override void StartPuzzle()
    {
        base.StartPuzzle();
        SetupCards();
        Utils.SetCameraInMiddleOfGrid(
            cardPrefab.transform.GetChild(0).GetComponent<Renderer>(),
            Camera.main,
            gridSize,
            border
            );
        Utils.SetCameraInMiddleOfBounds(boardBounds, Camera.main, border);
        board.transform.position = boardBounds.center;
        //board.size = new Vector2(gridSize.x + 0.5f, gridSize.y + 0.5f);
        board.size = boardBounds.size + boardScaleOffset;

        ResizeSpriteToScreen(Camera.main, spriteRenderer);
        ResizeSpriteToScreen(Camera.main, tutorial);

        ac = GameObject.FindGameObjectWithTag("AudioController").GetComponent<AudioController>();
    }

    private void SetupCards()
    {
        spawnedCards = new List<GameObject>();

       
        for (int card = 0; card < 2; card++)
        {
            
            for (int c = 0; c < sprites.Length; c++)
            {
                GameObject g = Instantiate(cardPrefab, Vector3.zero, Quaternion.identity);
                g.transform.SetParent(transform);
                g.transform.Find("Front").GetComponent<SpriteRenderer>().sprite = sprites[c];
                g.transform.name = sprites[c].name + "-" + card;
                g.GetComponent<MemoryCard>().InitCard();
                spawnedCards.Add(g);
            }
        }

        spawnedCards.Shuffle();

        int cardC = 0;
        Vector3 currentPos = Vector3.zero;
        boardBounds = new Bounds();
        Bounds cardbounds = spawnedCards[0].GetComponent<MemoryCard>().GetCardBounds();
        for (int y = 0; y < gridSize.y; y++)
        {
            currentPos = new Vector3(0, cardbounds.size.y * y, 0);
            for (int x = 0; x < gridSize.x; x++)
            {
                if (cardC < spawnedCards.Count && spawnedCards[cardC] != null)
                {
                    currentPos = new Vector3(cardbounds.size.x * x, currentPos.y, 0);
                    spawnedCards[cardC].transform.position = currentPos;

                    MemoryCard current = spawnedCards[cardC].GetComponent<MemoryCard>();
                    boardBounds.Encapsulate(current.Min.position);
                    boardBounds.Encapsulate(current.Max.position);
                    //spawnedCards[cardC].transform.position = new Vector3(x, y, 0);
                    cardC++;
                }
            }
        }
    }

    private void Update()
    {
        if(paused)
        {
            return;
        }
        if(Input.GetMouseButtonDown(0))
        {
            if (blockClicks)
            {
                Debug.Log("Clicks blocked");
                return;
            }
            MemoryCard selectedCard = GetCardAtPointerPos();
            ac.PlaySFX(onFlip);
            if (selectedCard != null && selectedCard != lastCard && !selectedCard.Showing)
            {
                blockClicks = true;
                selectedCard.ShowCard(() => {
                    CheckForPairs(selectedCard);
                });
            }
        }
        if (Input.GetKeyDown(KeyCode.KeypadDivide))
        {
            EndGame();
        }
    }

    public MemoryCard GetCardAtPointerPos()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        if (Physics2D.OverlapCircle(mousePos, 0.05f, cardsMask) == null)
        {
            Debug.Log("No collider clicked");
            return null;
        }
        return Physics2D.OverlapCircle(mousePos, 0.05f, cardsMask).GetComponent<MemoryCard>();
    }

    private void CheckForPairs(MemoryCard selectedCard)
    {
        if (lastCard == null)
        {
            Debug.Log("FIRST PICK");
            lastCard = selectedCard;
            blockClicks = false;
        }
        else
        {
            if (selectedCard.SpriteName == lastCard.SpriteName)
            {
                ac.PlaySFX(onCorrectGuess);
                Debug.Log("CORRECT!");
                selectedCard.Paired = true;
                lastCard.Paired = true;
                selectedCard.DisableCard();
                lastCard.DisableCard();
                lastCard = null;
                blockClicks = false;
                CheckForFinishCondition();
            }
            else
            {
                Debug.Log("WRONG!");
                ac.PlaySFX(onWrongGuess);
                WaitForSeconds(1f, () => {
                    selectedCard.HideCard(null);
                    lastCard.HideCard(() => {
                        lastCard = null;
                    });
                    blockClicks = false;
                });
            }
        }
    }

    private void CheckForFinishCondition()
    {
        bool completedGame = true;
        for (int i = 0; i < spawnedCards.Count; i++)
        {
            if(!spawnedCards[i].GetComponent<MemoryCard>().Paired)
            {
                completedGame = false;
            }
        }

        if(completedGame)
        {
            EndGame();
            //SceneManager.LoadScene(0);
        }
    }

    private void EndGame()
    {
        paused = true;
        ac.PlaySFX(onGameCompleted);
        Debug.Log("GAME COMPLETED!");
        transform.SetParent(board.transform);
        LeanTween.scale(board.gameObject, transform.localScale * 1.1f, 0.2f).setLoopPingPong(2).setOnComplete(() => { LoadNextLevel(); });
    }

    public void WaitForSeconds(float t, Action onWaitComplete)
    {
        StartCoroutine(Wait(t, onWaitComplete));
    }

    private IEnumerator Wait(float t, Action onWaitComplete)
    {
        yield return new WaitForSeconds(t);
        onWaitComplete?.Invoke();
    }

    public override void PauseGame()
    {
        Debug.Log("PAUSING GAME");
        paused = true;
        GameObject.FindGameObjectWithTag("UI").GetComponent<MenuController>().SwapMenu(1);
    }

    public override void ResumeGame()
    {
        Debug.Log("RESUMING GAME");
        GameObject.FindGameObjectWithTag("UI").GetComponent<MenuController>().SwapMenu(0);
        paused = false;
    }

    public override void RestartGame()
    {
        Debug.Log("RESETTING GAME");
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
    }

    public override void ShowTutorial(bool show)
    {
        Debug.Log("SHOWING TUTORIAL -> " + show);
        paused = show;
        tutorial.gameObject.SetActive(show);
    }
}
