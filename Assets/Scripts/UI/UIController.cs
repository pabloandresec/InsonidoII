using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIController : MenuController
{
    private AudioController ac;

    private void Start()
    {
        if(ac == null)
        {
            GameObject game = GameObject.FindGameObjectWithTag("AudioController");
            if(game != null)
            {
                ac = game.GetComponent<AudioController>();
            }
        }
    }

    public void LoadSceneAsync(int x)
    {
        SceneManager.LoadSceneAsync(x);
    }
    public void LoadSceneAsync(string name)
    {
        SceneManager.LoadSceneAsync(name);
    }

    public void RequestPause()
    {
        Puzzle currentgame = GameObject.FindGameObjectWithTag("GameController").GetComponent<Puzzle>();
        if(currentgame != null)
        {
            currentgame.PauseGame();
        }
    }

    public void RequestResume()
    {
        Puzzle currentgame = GameObject.FindGameObjectWithTag("GameController").GetComponent<Puzzle>();
        if (currentgame != null)
        {
            currentgame.ResumeGame();
        }
    }

    public void RequestRestart()
    {
        Puzzle currentgame = GameObject.FindGameObjectWithTag("GameController").GetComponent<Puzzle>();
        if (currentgame != null)
        {
            currentgame.RestartGame();
        }
    }

    public void RequestTutoria(bool show)
    {
        Puzzle currentgame = GameObject.FindGameObjectWithTag("GameController").GetComponent<Puzzle>();
        if (currentgame != null)
        {
            currentgame.ShowTutorial(show);
            if(show)
            {
                SwitchMenu(3);
            }
            else
            {
                SwitchMenu(0);
            }
        }
    }

    public void SetSFXVol(float val)
    {
        ac.SetSFXVol(val);
    }

    public void SetMusicVol(float val)
    {
        ac.SetMusicVol(val);
    }
}
