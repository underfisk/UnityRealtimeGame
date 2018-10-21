using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class XCustomList : MonoBehaviour
{
    /// <summary>
    /// Buttons to control the list of options
    /// </summary>
    private Button BackArrow, NextArrow;

    /// <summary>
    /// Target text to display the current option
    /// </summary>
    private Text TargetText;

    [SerializeField, Tooltip("Here we add the options as string and filter it equaly when going to save in playerprefs")]
    public String [] optionList;

    [SerializeField, Tooltip("Disables the buttons when that button is touched and has no more elements behind or after")]
    public bool DisableWhenZero = false;

    /// <summary>
    /// Starts at 0 in the List
    /// </summary>
    private int currentPosition = 0;

    protected void Start()
    {
        if (BackArrow == null) BackArrow = gameObject.transform.Find("BackArrow").GetComponent<Button>();
        if (NextArrow == null) NextArrow = gameObject.transform.Find("NextArrow").GetComponent<Button>();
        if (TargetText == null) TargetText = gameObject.transform.Find("Text").GetComponent<Text>();
        if (optionList.Length <= 0)
            Debug.LogError("[XCustomList] Please add some option in optionList");
        else//soon filter if he has some saved in prefabs so we load from there also set the current position according to what he has in the player prefs like find the index of that string in the array
            Display(optionList[0]);
        
        BackArrow.onClick.AddListener(() => OnArrowClick(false));
        NextArrow.onClick.AddListener(() => OnArrowClick(true));
    }

    /// <summary>
    /// TODO: Refactor
    /// </summary>
    /// <param name="next"></param>
    private void OnArrowClick(bool next = false)
    {
        if (next)
            currentPosition = (currentPosition++ >= optionList.Length) ? optionList.Length : currentPosition++;
        else
            currentPosition = (currentPosition-- <= 0 ) ? 0 : currentPosition--;

        if (currentPosition >= optionList.Length)
        {
            //Go to the last one
            Display(optionList[optionList.Length -1]);
        }
        else if (currentPosition <= 0)
        {
            //Go to the first one
            Display(optionList[0]);
        }
        else
        {
            //Go to new position
            Display(optionList[currentPosition]);
        }
    }

    /// <summary>
    /// Displays on text the option value
    /// TODO Refactor with dictionaries
    /// </summary>
    /// <param name="value"></param>
    private void Display(String value)
    {
        if (DisableWhenZero)
        {
            if (currentPosition >= optionList.Length)
            {
                if (!BackArrow.interactable) BackArrow.interactable = true;
                NextArrow.interactable = false;
            }
            else if (currentPosition <= 0)
            {
                if (!NextArrow.interactable) NextArrow.interactable = true;
                BackArrow.interactable = false;
            }
            else
            {
                BackArrow.interactable = true;
                BackArrow.interactable = true;
            }
        }
        TargetText.text = value;
    }
}
