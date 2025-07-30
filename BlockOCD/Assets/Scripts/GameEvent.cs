using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEvent : MonoBehaviour
{
    public static Action<bool> GameOver;

    public static Action<int> AddScores;

    public static Action CheckIfShapeCanBePlaced;
    public static Func<Shape,bool> CheckIfShapeCanBePlacedOnGrid;

    public static Action MoveShapeToStartPosition;

    public static Action ResquestNewShape;

    public static Action CheckIfPlayerLost;

    public static Action SetInactiveShape;

    public static Action<int, int> UpdateBestScore;

    public static Action<int, int> UpdateScorePopup;

    public static Action<Config.SquareColor> UpdateSquareColor;

    public static Action ShowWriting;

    public static Action<Config.SquareColor> ShowBonusWriting;
}
