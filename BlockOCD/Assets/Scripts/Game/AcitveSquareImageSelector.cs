using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AcitveSquareImageSelector : MonoBehaviour
{
    public SquareTextureData squareTextureData;
    public bool UpdateSquareColorOnReachTreshold = false;

    private void OnEnable()
    {
        UpdateActiveSquareColorBaseOnScore();

        if(UpdateSquareColorOnReachTreshold)
            GameEvent.UpdateSquareColor += UpdateSquareColor;
    }

    private void OnDisable()
    {
        GameEvent.UpdateSquareColor -= UpdateSquareColor;
    }

    private void UpdateActiveSquareColorBaseOnScore()
    {
        var squareTexture = squareTextureData.activeSquareTextures
            .Find(t => t.color == squareTextureData.currentColor);

        if (squareTexture != null)
            GetComponent<Image>().sprite = squareTexture.texture;
    }

    private void UpdateSquareColor(Config.SquareColor color)
    {
        if (this == null || gameObject == null) return;

        var squareTexture = squareTextureData.activeSquareTextures
            .Find(t => t.color == color);

        if (squareTexture != null)
            GetComponent<Image>().sprite = squareTexture.texture;
    }
}
