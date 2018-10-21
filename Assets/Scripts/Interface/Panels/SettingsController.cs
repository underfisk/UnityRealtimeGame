using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsController : MonoBehaviour
{
    public enum Tab
    {
        General,
        Resolution,
        Graphics
    }

    [Header("Content tab references")]
    [SerializeField]
    public GameObject GeneralContent, ResolutionContent, GraphicsContent;

    [Header("Bottom Actions")]
    [SerializeField]
    public Button SaveSettingsBtn, RestoreDefaultBtn, ExitBtn;

    [Header("Left Actions")]
    [SerializeField]
    public Button GeneralBtn, ResolutionBtn, GraphicsBtn; //etc...

    protected void Start()
    {
        //Notify if they are not defined 
        if (SaveSettingsBtn == null) Debug.LogError("Please add save settings button");
        if (RestoreDefaultBtn == null) Debug.LogError("Please add Restore default button");
        if (ExitBtn == null) Debug.LogError("Please add Exit button");
        if (GeneralBtn == null) Debug.LogError("Please add general button");
        if (ResolutionBtn == null) Debug.LogError("Please add Resolution button");
        if (GraphicsBtn == null) Debug.LogError("Please add Graphics button");
        if (GeneralContent == null) Debug.LogError("Please add GeneralContent");
        if (ResolutionContent == null) Debug.LogError("Please add ResolutionContent");
        if (GraphicsContent == null) Debug.LogError("Please add GraphicsContent");

        //Main 3 buttons bind
        if (ExitBtn != null) ExitBtn.onClick.AddListener(OnExitAction);
        if (SaveSettingsBtn != null) SaveSettingsBtn.onClick.AddListener(OnSaveAction);
        if (RestoreDefaultBtn != null) RestoreDefaultBtn.onClick.AddListener(OnRestoreAction);

        //Bind left bar 
        GeneralBtn.onClick.AddListener(() => SwapTab(Tab.General));
        ResolutionBtn.onClick.AddListener(() => SwapTab(Tab.Resolution));

        //Set the default which is general
        SwapTab(Tab.General);
    }

    private void OnExitAction()
    {
        //Here we compare the new options in every panel
        //and if some is different without being saved we ask him
        //if wants to save, discard or exit

        //Just for test also we need to verify in input if this window is open when we press escape

        //Soon optmizate and instead of hide we load and destroy if the interface comes to be heavy
        gameObject.SetActive(false);
    }

    private void OnSaveAction()
    {
        //Saves everything on every panel
    }

    private void OnRestoreAction()
    {
        //We make here the default values no problem
        //and we set it
        //some of them we can go directly on Unity, like default res, etc
    }

    /// <summary>
    /// Hides every tab open, just the content
    /// </summary>
    private void HideAllTabContent()
    {
        if (GeneralContent.activeSelf)
            GeneralContent.SetActive(false);
        if (ResolutionContent.activeSelf)
            ResolutionContent.SetActive(false);
        if (GraphicsContent.activeSelf)
            GraphicsContent.SetActive(false);
    }

    /// <summary>
    /// Changes the active tab and button
    /// </summary>
    /// <param name="_tab"></param>
    private void SwapTab(Tab _tab)
    {
        HideAllTabContent();
        switch(_tab)
        {
            case Tab.General:
                //Set the button with the active layer

                //Set the content opened
                GeneralContent.SetActive(true);
                break;
            case Tab.Resolution:
                //same as gen
                ResolutionContent.SetActive(true);
                break;
        }
    }
}
