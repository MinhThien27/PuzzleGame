using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu]
[System.Serializable]
public class SquareTextureData : ScriptableObject
{
    [System.Serializable]
    public class TextureData
    {
        public Sprite texture;
        public Config.SquareColor color;
    }

    public int tresholdValue = 10;
    private const int StartTresholdValue = 10;
    public List<TextureData> activeSquareTextures;

    //public Config.SquareColor defaultColor = Config.SquareColor.NotSet;
    public Config.SquareColor currentColor;
    private Config.SquareColor nextColor;

    private void Awake()
    {
        SetStartColor();
    }

    private void OnEnable()
    {
        SetStartColor();
    }

    public int GetCurrentSquareColorIndex()
    {
        int currentIndex = 0;
        for (var index = 0; index < activeSquareTextures.Count; index++)
        {
            if (activeSquareTextures[index].color == currentColor)
            {
                currentIndex = index;
            }
        }

        return currentIndex;
    }

    //Listed color selection
    //public void UpdateColor(int current_score)
    //{
    //    var currentColorIndex = GetCurrentSquareColorIndex();

    //    if (currentColorIndex == activeSquareTextures.Count - 1)
    //    {
    //        nextColor = activeSquareTextures[0].color;
    //    }
    //    else
    //    {
    //        nextColor = activeSquareTextures[currentColorIndex + 1].color;
    //    }

    //    currentColor = nextColor;

    //    tresholdValue = StartTresholdValue + current_score;
    //}

    //Random color selection
    public void UpdateColor(int current_score)
    {
        var currentColorIndex = GetCurrentSquareColorIndex();

        nextColor = activeSquareTextures[Random.Range(0,activeSquareTextures.Count)].color;

        currentColor = nextColor;

        tresholdValue = StartTresholdValue + current_score;
    }

    public void SetStartColor()
    {
        tresholdValue = StartTresholdValue;
        currentColor = activeSquareTextures[0].color;
        //nextColor = currentColor + 1; //test
    }
}