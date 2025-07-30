using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Grid : MonoBehaviour
{

    public ShapeStorage shapeStorage;
    public int columns = 0;
    public int rows = 0;
    public float squaresGap = 0.1f;
    public GameObject gridSquare;
    public Vector2 startPosition = new Vector2(0, 0);
    public float squareScale = 0.5f;
    public float everySquareOffset = 0f;
    public SquareTextureData squareTextureData;

    private Vector2 _offset = Vector2.zero;
    private List<GameObject> _gridSquares = new List<GameObject>();

    private LineIndicator _lineIndicator;

    private Config.SquareColor currentActiveSquareColor = Config.SquareColor.NotSet;

    private List<Config.SquareColor> colorsInTheGrid = new List<Config.SquareColor>();

    public int colorBonusScore = 50;


    private void OnEnable()
    {
        GameEvent.CheckIfShapeCanBePlaced += CheckIfShapeCanBePlaced;
        GameEvent.CheckIfShapeCanBePlacedOnGrid += CheckIfShapeCanBePlacedOnGrid;
        GameEvent.UpdateSquareColor += OnUpdateSquareColor;
        GameEvent.CheckIfPlayerLost += CheckIfPlayerLost;
    }

    private void OnDisable()
    {
        GameEvent.CheckIfShapeCanBePlaced -= CheckIfShapeCanBePlaced;
        GameEvent.UpdateSquareColor -= OnUpdateSquareColor;
        GameEvent.CheckIfPlayerLost -= CheckIfPlayerLost;
        GameEvent.CheckIfShapeCanBePlacedOnGrid -= CheckIfShapeCanBePlacedOnGrid;

    }

    void Start()
    {
        _lineIndicator = GetComponent<LineIndicator>();
        CreateGrid();

        currentActiveSquareColor = squareTextureData.activeSquareTextures[0].color;
    }

    private void OnUpdateSquareColor(Config.SquareColor color)
    {
        currentActiveSquareColor = color;
    }

    private List<Config.SquareColor> GetColorsInTheGrid()
    {
        
        var colors = new List<Config.SquareColor>();
        foreach (GameObject square in _gridSquares)
        {
            var gridSquare = square.GetComponent<GridSquare>();
            if (gridSquare.SquareOccupied && !colors.Contains(gridSquare.GetCurrentSquareColor()))
            {
                colors.Add(gridSquare.GetCurrentSquareColor());
            }
        }

        return colors;
    }

    private void CreateGrid()
    {
        SpawnGridSquares();
        SetGridSquaresPositions();
    }

    private void SpawnGridSquares()
    {
        //0, 1, 2, 3, 4
        //5, 6, 7, 8, 9

        int square_index = 0;
        for (var row = 0; row < rows; ++row)
        {
            for (var column = 0; column < columns; ++column)
            {
                _gridSquares.Add(Instantiate(gridSquare) as GameObject);
                _gridSquares[_gridSquares.Count - 1].GetComponent<GridSquare>().SquareIndex = square_index;
                //Set object vừa tạo làm con của object chứa script này cụ thể là Grid trên hierarchy
                _gridSquares[_gridSquares.Count - 1].transform.SetParent(this.transform);
                //Đặt tỉ lệ scale cho object vừa tạo
                _gridSquares[_gridSquares.Count - 1].transform.localScale = new Vector3(squareScale, squareScale, squareScale);
                _gridSquares[_gridSquares.Count - 1].GetComponent<GridSquare>().SetImage(_lineIndicator.GetGridSquareIndex(square_index) % 2 == 0);
                square_index++;
            }
        }

    }

    private void SetGridSquaresPositions()
    {
        int column_number = 0;
        int row_number = 0;
        Vector2 square_grap_number = Vector2.zero;
        bool row_moved = false;

        var square_rect = _gridSquares[0].GetComponent<RectTransform>();
        _offset.x = square_rect.rect.width * square_rect.transform.localScale.x + everySquareOffset;
        _offset.y = square_rect.rect.height * square_rect.transform.localScale.y + everySquareOffset;

        foreach (GameObject square in _gridSquares)
        {
            if (column_number + 1 > columns)
            {
                square_grap_number.x = 0;
                //xet den hang tiep theo
                column_number = 0;
                row_number++;
                row_moved = false;
            }

            var position_x_offset = _offset.x * column_number + (square_grap_number.x * squaresGap);
            var position_y_offset = _offset.y * row_number + (square_grap_number.y * squaresGap);

            if (column_number > 0 && column_number % 3 == 0)
            {
                square_grap_number.x++;
                position_x_offset += squaresGap;
            }

            if (row_number > 0 && row_number % 3 == 0 && row_moved == false)
            {
                row_moved = true;
                square_grap_number.y++;
                position_y_offset += squaresGap;
            }

            square.GetComponent<RectTransform>().anchoredPosition = new Vector2(startPosition.x + position_x_offset,
                                                                                startPosition.y - position_y_offset);
            square.GetComponent<RectTransform>().localPosition = new Vector3(startPosition.x + position_x_offset,
                                                                             startPosition.y - position_y_offset,
                                                                             0);

            column_number++;

        }

    }

    private void CheckIfShapeCanBePlaced()
    {

        var squareIndexes = new List<int>();

        foreach (GameObject square in _gridSquares)
        {
            var gridSquare = square.GetComponent<GridSquare>();
            if (gridSquare.Selected && !gridSquare.SquareOccupied)
            {
                squareIndexes.Add(gridSquare.SquareIndex);
                gridSquare.Selected = false;
                //gridSquare.ActiveSquare();
            }
        }

        var currentSelectedShape = shapeStorage.GetCurrentSelectedShape();
        if (currentSelectedShape == null)
        {
            Debug.Log("No shape selected to place");
            return;
        }

        if(currentSelectedShape.TotalSquaresNumber == squareIndexes.Count)
        {
            foreach (var squareIndex in squareIndexes)
            {
                _gridSquares[squareIndex].GetComponent<GridSquare>().PlaceShapeOnTheGrid(currentActiveSquareColor);
            }

            //AudioManager.Instance.PlaySFX("PlaceShape");

            var shapeLeft = 0;
            foreach (var shape in shapeStorage.shapeList)
            {
                if (shape.IsOnStartPosition() && shape.IsAnyOfShapeSquareActive())
                {
                    shapeLeft++;
                }
            }

            if (shapeLeft == 0)
            {
                GameEvent.ResquestNewShape();
            }
            else
            {
                GameEvent.SetInactiveShape();
            }

            CheckIfAnyLineIsCompleted();


        }
        else
        {
            GameEvent.MoveShapeToStartPosition();
        }
    }

    private void CheckIfAnyLineIsCompleted()
    {
        List<int[]> lines = new List<int[]>();

        //columns
        foreach (var column in _lineIndicator.columnIndexes)
        {
            lines.Add(_lineIndicator.GetVerticalLine(column));
        }

        //rows
        for (var row = 0; row < 9; row++)
        {
            List<int> data = new List<int>(9);
            for (var index = 0; index < 9; index++)
            {
                data.Add(_lineIndicator.line_data[row, index]);
            }

            lines.Add(data.ToArray());
        }

        //squares
        for (var square = 0; square < 9; square++)
        {
            List<int> data = new List<int>(9);
            for (var index = 0; index < 9; index++)
            {
                data.Add(_lineIndicator.square_data[square, index]);
            }
            lines.Add(data.ToArray());
        }

        //This func needs to be called before CheckIfSquaresAreCompleted
        colorsInTheGrid = GetColorsInTheGrid();

        var completedLines = CheckIfSquaresAreCompleted(lines);


        if(completedLines >= 2)
        {
            #if UNITY_ANDROID && !UNITY_EDITOR
                Handheld.Vibrate();
            #endif
            //TODO: Play bonus animation
            GameEvent.ShowWriting();
        }

        //TODO: Add Score
        var totalScore = 10 * completedLines;
        var bonusScore = PlayColorBonusAnimation(colorBonusScore);
        var finalScore = totalScore + bonusScore;

        GameEvent.AddScores(finalScore);
        GameEvent.CheckIfPlayerLost();
        //CheckIfPlayerLost();
    }

    private int PlayColorBonusAnimation(int bonusScore)
    {
        var colorsInTheGridAfterRomove = GetColorsInTheGrid();
        Config.SquareColor colorToPlayBonusFor = Config.SquareColor.NotSet;
        foreach (var color in colorsInTheGrid)
        {
            if(colorsInTheGridAfterRomove.Contains(color) == false)
            {
                colorToPlayBonusFor = color;
            }
        }

        //Should never play bonus for the current active square color
        if (colorToPlayBonusFor == Config.SquareColor.NotSet || colorToPlayBonusFor == currentActiveSquareColor)
        {
            Debug.Log("No color bonus to play, all colors are still in the grid");
            return 0; // Return 0 to indicate no bonus was played
        }
        else
        {
            GameEvent.ShowBonusWriting(colorToPlayBonusFor);
            #if UNITY_ANDROID && !UNITY_EDITOR
                Handheld.Vibrate();
            #endif
            return bonusScore; // Return 50 to indicate a bonus was played
        }
    }

    private int CheckIfSquaresAreCompleted(List<int[]> data)
    {
        List<int[]> completedLines = new List<int[]>();

        var linesCompletd = 0;

        foreach (var line in data)
        {
            var lineCompleted = true;

            foreach (var squareIndex in line)
            {
                var comp = _gridSquares[squareIndex].GetComponent<GridSquare>();
                if (!comp.SquareOccupied)
                {
                    lineCompleted = false;
                }
            }

            if (lineCompleted)
            {
                //linesCompletd++;
                completedLines.Add(line);
            }
        }

        foreach (var line in completedLines)
        {
            var completed = false;

            foreach (var squareIndex in line)
            {
                var comp = _gridSquares[squareIndex].GetComponent<GridSquare>();
                comp.Deactivate();
                completed = true;
            }

            foreach (var squareIndex in line)
            {
                var comp = _gridSquares[squareIndex].GetComponent<GridSquare>();
                comp.ClearOccupied();
            }

            if (completed)
            {
                linesCompletd++;
            }
        }

        Debug.Log($"Lines completed: {linesCompletd}");
        return linesCompletd;
    }

    private void CheckIfPlayerLost()
    {
        var validShapes = 0;

        for(var index = 0; index < shapeStorage.shapeList.Count; index++)
        {
            var isShapeActive = shapeStorage.shapeList[index].IsAnyOfShapeSquareActive();
            if (GameEvent.CheckIfShapeCanBePlacedOnGrid(shapeStorage.shapeList[index]) && isShapeActive)
            {
                shapeStorage.shapeList[index]?.ActivateShape();
                validShapes++;
            }
        }

        if(validShapes == 0)
        {
            //GAMEOVER
            Debug.Log("Player lost, no valid shapes to place on the grid");
            AudioManager.Instance.PlaySFX("GameOver");
            GameEvent.GameOver(false);
        }
        else
        {
            Debug.Log($"Player has {validShapes} valid shapes to place on the grid");
        }
    }

    private bool CheckIfShapeCanBePlacedOnGrid(Shape currentShape)
    {
        var currentShapeData = currentShape.CurrentShapeData;
        var shapeColums = currentShapeData.columns;
        var shapeRows = currentShapeData.rows;

        //All indexes of filled up square
        List<int> originalShapeFilledUpSquares = new List<int>();
        var squareIndex = 0;

        for (var rowIndex = 0; rowIndex < shapeRows; rowIndex++)
        {
            for (var columnIndex = 0; columnIndex < shapeColums; columnIndex++)
            {
                if (currentShapeData.board[rowIndex].column[columnIndex])
                {
                    originalShapeFilledUpSquares.Add(squareIndex);
                }
                squareIndex++;
            }
        }

        if (currentShape.TotalSquaresNumber != originalShapeFilledUpSquares.Count)
        {
            Debug.Log("Shape is not valid, not enough squares to place on the grid");
        }

        var squareList = GetAllSquaresCombination(shapeColums, shapeRows);

        bool shapeCanBePlaced = false;

        foreach (var number in squareList)
        {
            bool shapeCanBePlacedOnBoard = true;
            foreach (var squareIndexToCheck in originalShapeFilledUpSquares)
            {
                var comp = _gridSquares[number[squareIndexToCheck]].GetComponent<GridSquare>();

                if (comp.SquareOccupied)
                {
                    shapeCanBePlacedOnBoard = false;
                    break;
                }
                else
                {
                    shapeCanBePlacedOnBoard = true;
                }
            }

            if (shapeCanBePlacedOnBoard)
            {
                shapeCanBePlaced = true;
            }
        }

        return shapeCanBePlaced;
    }


    private List<int[]> GetAllSquaresCombination(int colums, int rows)
    {
        var squareList = new List<int[]>();
        var lastColumnIndex = 0;
        var lastRowIndex = 0;

        int safeIndex = 0;

        while(lastRowIndex + (rows -1) < 9)
        {
            var rowData = new List<int>();

            for (var row = lastRowIndex; row < lastRowIndex + rows; row++)
            {
                for (var column = lastColumnIndex; column < lastColumnIndex + colums; column++)
                {
                    rowData.Add(_lineIndicator.line_data[row, column]);
                }
            }

            squareList.Add(rowData.ToArray());
            lastColumnIndex++;
             if (lastColumnIndex + (colums - 1) >= 9)
            {
                lastRowIndex++;
                lastColumnIndex = 0;
            }

             safeIndex++;

            if(safeIndex > 100) // Prevent infinite loop
            {
                Debug.LogError("Infinite loop detected in GetAllSquaresCombination");
                break;
            }
        }

        return squareList;
    }
}
