using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridSquare : MonoBehaviour
{
    public Image hooverImage;
    public Image activeImage;
    public Image normalImage;
    public List<Sprite> normalImages;

    private Config.SquareColor currentSquareColor = Config.SquareColor.NotSet;
    public Config.SquareColor GetCurrentSquareColor()
    {
        return currentSquareColor;
    }

    public bool Selected { get; set; }
    public int SquareIndex { get; set; }
    public bool SquareOccupied { get; set; }
      
    void Start()
    {
        Selected = false;
        SquareOccupied = false;
    }

    //temp function (remove later)
    public bool CanUseThisSquare()
    {
        return hooverImage.gameObject.activeSelf;
    }

    public void PlaceShapeOnTheGrid(Config.SquareColor color)
    {
        currentSquareColor = color;
        ActiveSquare();
    }

    public void ActiveSquare()
    {
        hooverImage.gameObject.SetActive(false);
        activeImage.gameObject.SetActive(true);
        Selected = true;
        SquareOccupied = true;

    }

    public void Deactivate()
    {
        currentSquareColor = Config.SquareColor.NotSet;
        activeImage.gameObject.SetActive(false);
    }

    public void ClearOccupied()
    {
        currentSquareColor = Config.SquareColor.NotSet;
        Selected = false;
        SquareOccupied = false;
    }

    public void SetImage(bool setFirstImage)
    {
        //xet xem neu image dau da dc set thi set bang image tiep theo
        normalImage.GetComponent<Image>().sprite = setFirstImage ? normalImages[1] : normalImages[0];
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(!SquareOccupied)
        {
            Selected = true;
            hooverImage.gameObject.SetActive(true);
        }
        else if(collision.gameObject.GetComponent<ShapeSquare>() != null)
        {
            collision.gameObject.GetComponent<ShapeSquare>().SetOccupiedImage();
        }
    }


    private void OnTriggerStay2D(Collider2D collision)
    {
        Selected = true;

        if (!SquareOccupied)
        {
            hooverImage.gameObject.SetActive(true);
        }
        else if (collision.gameObject.GetComponent<ShapeSquare>() != null)
        {
            collision.gameObject.GetComponent<ShapeSquare>().SetOccupiedImage();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!SquareOccupied)
        {
            Selected = false;
            hooverImage.gameObject.SetActive(false);
        }
        else if (collision.gameObject.GetComponent<ShapeSquare>() != null)
        {
            collision.gameObject.GetComponent<ShapeSquare>().UnsetOccupiedImage();
        }
    }
}
