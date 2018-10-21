using Assets.Scripts.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class XCustomResolutions : MonoBehaviour
{
    /// <summary>
    /// Possible monitor resolutions
    /// </summary>
    protected Resolution[] possibleResolutions;

    /// <summary>
    /// Starts at 0 in the List
    /// </summary>
    private int currentPosition = 0;

    /// <summary>
    /// Buttons to control the list of options
    /// </summary>
    private Button BackArrow, NextArrow;

    /// <summary>
    /// Target text to display the current option
    /// </summary>
    private Text TargetText;

    [SerializeField, Tooltip("Disables the buttons when that button is touched and has no more elements behind or after")]
    public bool DisableWhenZero = false;

    protected void Start()
    {
        if (BackArrow == null) BackArrow = gameObject.transform.Find("BackArrow").GetComponent<Button>();
        if (NextArrow == null) NextArrow = gameObject.transform.Find("NextArrow").GetComponent<Button>();
        if (TargetText == null) TargetText = gameObject.transform.Find("Text").GetComponent<Text>();

        BackArrow.onClick.AddListener(() => OnArrowClick(false));
        NextArrow.onClick.AddListener(() => OnArrowClick(true));

        //TODO: Fix the bug of going from the max res or meaning the atual one
        //to a other one we dont know
        possibleResolutions = ScreenController.GetPossibleResolutions();
        Display(ScreenController.CurrentResolution());
    }


    /// <summary>
    /// TODO: Refactor
    /// </summary>
    /// <param name="next"></param>
    private void OnArrowClick(bool next = false)
    {
        if (next)
            currentPosition = (currentPosition++ >= possibleResolutions.Length) ? possibleResolutions.Length : currentPosition++;
        else
            currentPosition = (currentPosition-- <= 0) ? 0 : currentPosition--;

        if (currentPosition >= possibleResolutions.Length)
        {
            //Go to the last one
            Display(possibleResolutions[possibleResolutions.Length - 1]);
        }
        else if (currentPosition <= 0)
        {
            //Go to the first one
            Display(possibleResolutions[0]);
        }
        else
        {
            //Go to new position
            Display(possibleResolutions[currentPosition]);
        }
    }

    /// <summary>
    /// Displays on text the option value
    /// TODO Refactor with dictionaries
    /// </summary>
    /// <param name="value"></param>
    private void Display(Resolution res)
    {
        if (DisableWhenZero)
        {
            if (currentPosition >= possibleResolutions.Length)
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
        TargetText.text = res.ToString();
    }

}
