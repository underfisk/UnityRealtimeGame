using Assets.Scripts.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class InputController : MonoBehaviour
{
    /// <summary>
    /// Temporary just for not being printing the whole time
    /// </summary>
    [SerializeField]
    public RuntimePlatform runningPlatform;

    /// <summary>
    /// Temporary to debug
    /// </summary>
    [SerializeField]
    public Scene runningScene;

    /// <summary>
    /// Initializes input needed information
    /// </summary>
    protected void Start()
    {
        runningPlatform = Application.platform;
        runningScene = SceneManager.GetActiveScene();
    }

    /// <summary>
    /// Called each frame to control the user input
    /// </summary>
    protected void Update()
    {
        GetKeys();
    }

    /// <summary>
    /// Verifies which scene is running and filters the key that get pressed
    /// by opening that exception by scene
    /// </summary>
    private void GetKeys()
    {
        if (runningScene.name == "Login")
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                var usr_field = GameObject.Find("username_field").GetComponent<InputField>();
                var pwd_field = GameObject.Find("password_field").GetComponent<InputField>();

                if (usr_field != null && usr_field.isFocused == true)
                    pwd_field.Select();
                else
                    usr_field.Select();
            }
            else if (Input.GetKeyDown(KeyCode.Return))
            {
                var loginComponent = GameObject.Find("LoginHandler").GetComponent<Login>();
                if (loginComponent != null && loginComponent.canSubmit)
                {
                    loginComponent.SubmitValidation();
                }
            }
        }

        if (runningScene.name == "MainMenu")
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                //Close modals if we have some before render
                var listOfModals = GameObject.FindGameObjectsWithTag("Modal");
                foreach (var _modal in listOfModals)
                {
                    if (_modal.gameObject.activeSelf)
                    {
                        //Invoke if exists
                        var closeBtn = _modal.transform.Find("CloseModal");
                        if (closeBtn != null)
                            closeBtn.GetComponent<Button>().onClick.Invoke();

                        //Invoke if exists
                        var cancelBtn = _modal.transform.Find("CancelModal");
                        if (cancelBtn != null)
                            cancelBtn.GetComponent<Button>().onClick.Invoke();

                        return;
                    }
                }

                //Do we have some special scene like settings open?
                var listOfPanels = GameObject.FindGameObjectsWithTag("Panel");
                foreach (var _panel in listOfPanels)
                {
                    if (_panel.gameObject.activeSelf)
                    {
                        _panel.SetActive(false); //we can destroy after if we go to instantiate/destroy version
                        return;
                    }
                }

                //Do we have already a quit scene?
                var quitWindow = GameObject.Find("Modal_QuitGame");
                if (quitWindow != null)
                    Destroy(quitWindow);
                else
                {
                    //Instantiate the modal
                    var modal = new Modal("QUIT", "Do you want to quit?", Modal.Type.QuitGame);
                    modal.AddConfirmListener(() => Application.Quit());
                    modal.AddCancelListener(() => Destroy(modal.GetReference()));
                    modal.Render();
                }
            }
        }

        if (runningScene.name == "BattleTable")
        {

        }

    }
}

