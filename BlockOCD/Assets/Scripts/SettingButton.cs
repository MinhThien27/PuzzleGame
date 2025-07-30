using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SettingButton : MonoBehaviour
{
    public Button openSettingButton;
    public Button closeSettingButton;

    //public void SettingOpened()
    //{
    //    openSettingButton.gameObject.SetActive(false);
    //    closeSettingButton.gameObject.SetActive(true);
    //    closeSettingButton.interactable = true;

    //}

    public void SettingOpenedAndClosed()
    {
        openSettingButton.interactable = !openSettingButton.interactable;
        closeSettingButton.interactable = !closeSettingButton.interactable;
        openSettingButton.gameObject.SetActive(!openSettingButton.IsActive());
        closeSettingButton.gameObject.SetActive(!closeSettingButton.IsActive());
    }

}
