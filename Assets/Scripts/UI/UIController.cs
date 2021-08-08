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
}
