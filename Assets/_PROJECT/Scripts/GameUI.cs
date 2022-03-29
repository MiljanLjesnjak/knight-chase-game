using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{

    private void Start()
    {
        StartCoroutine(scoreUpdate());
    }

    [SerializeField]
    Text bestscoreText = default, scoreText = default;

    IEnumerator scoreUpdate()
    {
        while (true)
        {
            scoreText.text = KnightController.score.ToString();

            yield return new WaitForSeconds(0.25f);
        }
    }

    //pausePanel buttons
    public void restartGame()
    {
        SceneManager.LoadScene(1);
    }

    public void returntoMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }


    //pausePanel
    [SerializeField]
    GameObject pausePanel = default;

    public void open_pause_panel()
    {
        PlayerPrefs.SetInt("bestScore", KnightController.best_score);
        bestscoreText.text = KnightController.best_score.ToString();

        pausePanel.SetActive(true);
    }

    public void close_pause_panel()
    {
        pausePanel.SetActive(false);
    }

    void OnApplicationQuit()
    {
        PlayerPrefs.SetInt("bestScore", KnightController.best_score);
    }

}

