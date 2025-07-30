using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

[System.Serializable]
public class BestScoreData
{
    public int score = 0;
}

public class Scores : MonoBehaviour
{
    public static Scores Instance { get; private set; }

    public SquareTextureData squareTextureData;

    public Text scores;

    private bool isNewBestScore = false;
    private BestScoreData bestScore = new BestScoreData();

    public int _previousBestScore = 0;
    public int _currentScore = 0;

    private string bestScoreKey = "BestScore";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (BinaryDataStream.Exist(bestScoreKey))
        {
            StartCoroutine(ReadDataFile());
        }
    }


    private IEnumerator ReadDataFile()
    {
        bestScore = BinaryDataStream.Read<BestScoreData>(bestScoreKey);
        
        _previousBestScore = bestScore.score;

        Debug.Log("Previous Best Score: " + _previousBestScore);

        yield return new WaitForEndOfFrame();

        GameEvent.UpdateBestScore(_currentScore, bestScore.score);
        Debug.Log("Best Score: " + bestScore.score);
    }

    void Start()
    {
        _currentScore = 0;
        isNewBestScore = false;

        squareTextureData.SetStartColor();

        UpdateScoresText();
    }

    private void OnEnable()
    {
        GameEvent.AddScores += AddScores;
        GameEvent.GameOver += SaveBestScores;
    }

    private void OnDisable()
    {
        GameEvent.AddScores -= AddScores;
        GameEvent.GameOver -= SaveBestScores;
    }

    public void SaveBestScores(bool newBestScore)
    {
        if (isNewBestScore)
        {
            bestScore.score = _currentScore;
            BinaryDataStream.Save<BestScoreData>(bestScore, bestScoreKey);
        }
    }

    private void AddScores(int score)
    {
        _currentScore += score;

        if (score > 0)
        { 
            AudioManager.Instance.PlaySFX("AddScore");
        }

        if (_currentScore > bestScore.score)
        {
            isNewBestScore = true;
            bestScore.score = _currentScore;
        }

        GameEvent.UpdateBestScore(_currentScore, bestScore.score);

        UpdateScoresText();

        if(_currentScore >= squareTextureData.tresholdValue)
            UpdateSquareColor();
    }

    private void UpdateSquareColor()
    {
        if( GameEvent.UpdateSquareColor != null)
        {
            squareTextureData.UpdateColor(_currentScore);
            GameEvent.UpdateSquareColor(squareTextureData.currentColor);
        }
    }
    private void UpdateScoresText()
    {
        scores.text = _currentScore.ToString();
    }
}
