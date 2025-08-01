﻿using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class Shape : MonoBehaviour, IPointerClickHandler, IPointerUpHandler, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public GameObject squareShapeImage;
    public Vector3 shapeSelectedScale;
    public Vector2 offset = new Vector2(0, 100);

    [HideInInspector]
    public ShapeData CurrentShapeData;

    public int TotalSquaresNumber { get; set; }

    private List<GameObject> _currentShape = new List<GameObject>();
    private Vector3 _shapeStartScale;
    private RectTransform _transform;
    private bool _shapeDraggable = true;
    private Canvas _canvas;
    private Vector3 _startPosition;
    private bool _shapeActive = true;

    private Vector2 _dragOffset;
    private Vector2 _defaultAnchorMin;
    private Vector2 _defaultAnchorMax;
    private Vector2 _defaultPivot;

    private void Awake()
    {
        _transform = GetComponent<RectTransform>();

        _shapeStartScale = _transform.localScale;
        _startPosition = _transform.localPosition;

        _defaultAnchorMin = _transform.anchorMin;
        _defaultAnchorMax = _transform.anchorMax;
        _defaultPivot = _transform.pivot;

        _canvas = GetComponentInParent<Canvas>();
    }

    void Start()
    {
        //RequestNewShape(CurrentShapeData);
    }

    private void OnEnable()
    {
        GameEvent.MoveShapeToStartPosition += MoveShapeToStartPosition;
        GameEvent.SetInactiveShape += SetInactiveShape;
    }

    private void OnDisable()
    {
        GameEvent.MoveShapeToStartPosition -= MoveShapeToStartPosition;
        GameEvent.SetInactiveShape -= SetInactiveShape;
    }

    public bool IsOnStartPosition()
    {
        return _transform.localPosition == _startPosition;
    }

    public bool IsAnyOfShapeSquareActive()
    {
        foreach (var square in _currentShape)
        {
            if (square.gameObject.activeSelf)
                return true;
        }
        return false;
    }

    public void DeactivateShape()
    {
        if(_shapeActive)
        {
            foreach (var square in _currentShape)
            {
                square?.GetComponent<ShapeSquare>().DeactivateShapeSquare();
            }
        }
        _shapeActive = false;
    }

    public void SetInactiveShape()
    {
        if(!IsOnStartPosition() && IsAnyOfShapeSquareActive())
        {
            foreach (var square in _currentShape)
            {
                square.gameObject.SetActive(false);
            }
        }
    }


    public void ActivateShape()
    {
        if (!_shapeActive)
        {
            foreach (var square in _currentShape)
            {
                square?.GetComponent<ShapeSquare>().ActivateShapeSquare();
            }
        }
        _shapeActive = true;
    }

    public void RequestNewShape(ShapeData shapeData)
    {
        _transform.localPosition = _startPosition;
        //if (CurrentShapeData != null && CurrentShapeData == shapeData)
        //    return;
        CreateShape(shapeData);
    }

    public void CreateShape(ShapeData shapeData)
    {
        CurrentShapeData = shapeData;
        TotalSquaresNumber = GetNumberOfSquares(shapeData);

        while(_currentShape.Count <= TotalSquaresNumber)
        {
            _currentShape.Add(Instantiate(squareShapeImage, transform) as GameObject);
        }

        foreach( var square in _currentShape)
        {
            square.gameObject.transform.position = Vector3.zero;
            square.gameObject.SetActive(false);
        }

        var squareRect = squareShapeImage.GetComponent<RectTransform>();
        var moveDistance = new Vector2(squareRect.rect.width * squareRect.localScale.x, squareRect.rect.height * squareRect.localScale.y);

        int currentIndexInList = 0;

        //set pos to form final shape
        for (var row = 0; row < shapeData.rows; row++)
        {
            for (var col = 0; col < shapeData.columns; col++)
            {
                if (shapeData.board[row].column[col])
                {
                    _currentShape[currentIndexInList].SetActive(true);
                    _currentShape[currentIndexInList].GetComponent<RectTransform>().localPosition 
                                 = new Vector2(GetXPositionForShapeSquare(shapeData,col,moveDistance),
                                               GetYPositionForShapeSquare(shapeData,row,moveDistance));

                    currentIndexInList++;
                }
            }
        }
    }

    private float GetXPositionForShapeSquare(ShapeData shapeData, int column, Vector2 moveDistance)
    {
        float shiftOnX = 0f;
        if (shapeData.columns > 1)
        {
            float startXPos;
            if (shapeData.columns % 2 != 0)
                startXPos = (shapeData.columns / 2) * moveDistance.x * -1;
            else
                startXPos = ((shapeData.columns / 2) - 1) * moveDistance.x * -1 - moveDistance.x / 2;
            shiftOnX = startXPos + column * moveDistance.x;
        }
        return shiftOnX;
    }

    private float GetYPositionForShapeSquare(ShapeData shapeData, int row, Vector2 moveDistance)
    {
        float shiftOnY = 0f;
        if (shapeData.rows > 1)
        {
            float startYPos;
            if (shapeData.rows % 2 != 0)
                startYPos = (shapeData.rows / 2) * moveDistance.y;
            else
                startYPos = ((shapeData.rows / 2) - 1) * moveDistance.y + moveDistance.y / 2;
            shiftOnY = startYPos - row * moveDistance.y;
        }
        return shiftOnY;
    }

    private int GetNumberOfSquares(ShapeData shapeData)
    {
        int number = 0;

        foreach (var rowData in CurrentShapeData.board)
        {
            foreach (var active in rowData.column)
            {
                if (active)
                {
                    number++;
                }
            }
        }

        return number;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //throw new System.NotImplementedException();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        //throw new System.NotImplementedException();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        //throw new System.NotImplementedException();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        this.GetComponent<RectTransform>().localScale = shapeSelectedScale;

        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _canvas.transform as RectTransform,
            eventData.position,
            Camera.main,
            out localPoint
        );

        _dragOffset = _transform.localPosition - (Vector3)localPoint;
    }

    public void OnDrag(PointerEventData eventData)
    {
        _transform.anchorMin = Vector2.zero;
        _transform.anchorMax = Vector2.zero;
        _transform.pivot = Vector2.zero;

        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _canvas.transform as RectTransform,
            eventData.position,
            Camera.main,
            out pos
        );

        _transform.localPosition = pos + offset + _dragOffset;
    }


    public void OnEndDrag(PointerEventData eventData)
    {
        _transform.localScale = _shapeStartScale;

        _transform.anchorMin = _defaultAnchorMin;
        _transform.anchorMax = _defaultAnchorMax;
        _transform.pivot = _defaultPivot;

        GameEvent.CheckIfShapeCanBePlaced();
    }

    private void MoveShapeToStartPosition()
    {
        _transform.localPosition = _startPosition;
        _transform.localScale = _shapeStartScale;
    }
}
