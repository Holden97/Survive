using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    private void Awake()
    {
        Cursor.visible = true;
        Time.timeScale = 1;
    }
    public void PlayGame()
    {
        Invoke("PlayGame2", 1f);
    }

    private void PlayGame2()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        Time.timeScale = 1;
        if (GameManager.Instance != null)
            GameManager.Instance.curGameIsOver = false;
    }

    public void QuitGame()
    {
        Application.Quit();
    }


    public void UIEnable()
    {
        GameObject.Find("Canvas/Menu/UI").SetActive(true);
    }
}
