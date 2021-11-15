using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIController : MenuController
{
    

    public void LoadSceneAsync(int x)
    {
        SceneManager.LoadSceneAsync(x);
    }
    public void LoadSceneAsync(string name)
    {
        SceneManager.LoadSceneAsync(name);
    }

    public void RequestStartGame()
    {
        Puzzle currentgame = GameObject.FindGameObjectWithTag("GameController").GetComponent<Puzzle>();
        if (currentgame != null)
        {
            if(!currentgame.GameStarted)
            {
                currentgame.StartPuzzle();
            }
            else
            {
                RequestTutoria(false);
            }
        }
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
}
