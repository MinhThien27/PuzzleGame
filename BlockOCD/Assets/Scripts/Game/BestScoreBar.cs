using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BestScoreBar : MonoBehaviour
{
    public Image fillInImage;
    public Text bestScoreText;

    void Start()
    {
        
    }

    private void OnEnable()
    {
        GameEvent.UpdateBestScore += UpdateBestScore;
    }

    private void OnDisable()
    {
        GameEvent.UpdateBestScore -= UpdateBestScore;
    }

    private void UpdateBestScore(int currentScore, int bestScore)
    {
        if (currentScore > bestScore)
        {
            bestScoreText.text = currentScore.ToString();
            fillInImage.fillAmount = 1f;
        }
        else
        {
            bestScoreText.text = bestScore.ToString();
            fillInImage.fillAmount = (float)currentScore / bestScore;
        }
    }

}
