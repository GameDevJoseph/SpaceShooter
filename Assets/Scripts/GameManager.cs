using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] bool _isGameOver;

    public void GameOver()
    {
        _isGameOver = true;
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) && _isGameOver)
            SceneManager.LoadScene("Game");

        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
    }
}
