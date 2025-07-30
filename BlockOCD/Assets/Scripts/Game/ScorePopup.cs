using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ScorePopup : MonoBehaviour
{
    public Image scoreWritting;
    public Text scoreText;

    private void Start()
    {
        //scoreWritting.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        GameEvent.UpdateScorePopup += UpdateScorePopup;
    }

    private void OnDisable()
    {
        GameEvent.UpdateScorePopup -= UpdateScorePopup;
    }

    private void UpdateScorePopup(int currentScore, int previousBestScore)
    {
        scoreWritting.gameObject.SetActive(currentScore > previousBestScore);
        scoreText.text = currentScore.ToString();
    }
}
