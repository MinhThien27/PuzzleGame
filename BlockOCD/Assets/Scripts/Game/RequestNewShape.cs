using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class RequestNewShape : MonoBehaviour
{
    //public int numberOfRequests;
    public Text numberText;

    private int _currentNumberOfRequests;
    private Button _button;
    private bool _isLocked;

    private void SetNumberOfRequest(int number)
    {
        PlayerPrefs.SetInt("NumberOfRequest", number);
        PlayerPrefs.Save();
    }

    private void Start()
    {
        // For test
        //SetNumberOfRequest(numberOfRequests);


        _currentNumberOfRequests = PlayerPrefs.GetInt("NumberOfRequest");
        _button = GetComponent<Button>();
        _button.onClick.AddListener(OnButtonClick);

        //Give 1 request per day
        GiveDailyRequest();

        if(_currentNumberOfRequests > 0)
        {
            UnLock();
        }
        else
        {
            Lock();
        }
    }

    private void OnButtonClick()
    { 
        if (!_isLocked && _currentNumberOfRequests > 0)
        {
            _currentNumberOfRequests--;
            numberText.text = _currentNumberOfRequests.ToString();
            GameEvent.ResquestNewShape();
            GameEvent.CheckIfPlayerLost();

            PlayerPrefs.SetInt("NumberOfRequest", _currentNumberOfRequests);
            PlayerPrefs.Save();

            if(_currentNumberOfRequests <= 0)
            {
                Lock();
            }
        }
        else
        {
            Lock();
        }
    }

    private void Lock()
    {
        _isLocked = true;
        _button.interactable = false;
        numberText.text = _currentNumberOfRequests.ToString();
    }

    private void UnLock()
    {
        _isLocked = false;
        _button.interactable = true;
        numberText.text = _currentNumberOfRequests.ToString();
    }

    private void GiveDailyRequest()
    {
        string lastDateStr = PlayerPrefs.GetString("LastDailyRequestDate", "");
        DateTime today = DateTime.Today;

        if (!string.IsNullOrEmpty(lastDateStr))
        {
            DateTime lastDate = DateTime.Parse(lastDateStr);
            if (lastDate.Date == today)
            {
                return;
            }
        }

        _currentNumberOfRequests++;
        PlayerPrefs.SetInt("NumberOfRequest", _currentNumberOfRequests);
        PlayerPrefs.Save();

        PlayerPrefs.SetString("LastDailyRequestDate", today.ToString("yyyy-MM-dd"));
        PlayerPrefs.Save();

        numberText.text = _currentNumberOfRequests.ToString();
        UnLock();
    }
}
