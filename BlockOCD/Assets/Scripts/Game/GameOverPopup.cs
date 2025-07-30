using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverPopup : MonoBehaviour
{
    public GameObject gameOverWindow;
    public GameObject gameOverPopup;
    public GameObject scorePopup;

    void Start()
    {
        gameOverWindow.SetActive(false);
    }

    private void OnEnable()
    {
        GameEvent.GameOver += HandleGameOver;
    }

    private void OnDisable()
    {
        GameEvent.GameOver -= HandleGameOver;
    }

    private void HandleGameOver(bool score)
    {
        StartCoroutine(OnGameOver(score));
    }

    private IEnumerator OnGameOver(bool score)
    {

        Debug.Log("Your score: " + Scores.Instance._currentScore);
        Debug.Log("Best score: " + Scores.Instance._previousBestScore);
        yield return new WaitForSeconds(2f); // Wait for 1 second before showing the game over popup

        gameOverWindow.SetActive(true);
        gameOverPopup.SetActive(true);
        scorePopup.SetActive(true);

        GameEvent.UpdateScorePopup(Scores.Instance._currentScore, Scores.Instance._previousBestScore);
    }
}
