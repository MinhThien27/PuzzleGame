using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapeStorage : MonoBehaviour
{
    public List<ShapeData> shapeData;
    public List<Shape> shapeList;

    private bool hasPlacableShape = false;

    void Start()
    {
        foreach (var shape in shapeList)
        {
            var shapeIndex = UnityEngine.Random.Range(0, shapeData.Count);
            shape.CreateShape(shapeData[shapeIndex]);
        }
    }

    private void OnEnable()
    {
        GameEvent.ResquestNewShape += RequestNewShape;
    }

    private void OnDisable()
    {
        GameEvent.ResquestNewShape -= RequestNewShape;
    }

    public Shape GetCurrentSelectedShape()
    {
        foreach (var shape in shapeList)
        {
            if (!shape.IsOnStartPosition() && shape.IsAnyOfShapeSquareActive())
            {
                return shape;
            }
        }

        Debug.Log("No shape selected");
        return null;
    }

    private void RequestNewShape()
    {
        //foreach (var shape in shapeList)
        //{
        //    var shapeIndex = UnityEngine.Random.Range(0, shapeData.Count);

        //    shape.RequestNewShape(shapeData[shapeIndex]);
        //}


        // Every request new shape, there is at least 1 shape that can be placed on the grid
        int maxTries = 10;
        int tries = 0;

        do
        {
            hasPlacableShape = false;

            foreach (var shape in shapeList)
            {
                var shapeIndex = UnityEngine.Random.Range(0, shapeData.Count);
                shape.RequestNewShape(shapeData[shapeIndex]);

                bool canPlace = GameEvent.CheckIfShapeCanBePlacedOnGrid?.Invoke(shape) ?? false;

                if (canPlace)
                {
                    hasPlacableShape = true;
                    shape.ActivateShape();
                }
                else
                {
                    shape.SetInactiveShape();
                }
            }

            tries++;
        } while (!hasPlacableShape && tries < maxTries);

        if(!hasPlacableShape && tries < maxTries)
        {
            GameEvent.GameOver?.Invoke(false);
        }
    }
}
