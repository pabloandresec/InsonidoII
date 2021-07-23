using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{
    public void LoadScene(int x)
    {
        SceneManager.LoadScene(x);
    }
    public void LoadSceneAsync(int x)
    {
        SceneManager.LoadSceneAsync(x);
    }
}
