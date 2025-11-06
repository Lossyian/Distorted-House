using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    [SerializeField] private string gameSceneName = "PlayScene";

    [SerializeField] private Button startButton;
    [SerializeField] private Button exitButton;

    private void Awake()
    {
        if (startButton != null)
            startButton.onClick.AddListener(StartGame);

        if (exitButton != null)
            exitButton.onClick.AddListener(QuitGame);
    }
    private void StartGame()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    private void QuitGame()
    {
        Application.Quit();
    }

}
