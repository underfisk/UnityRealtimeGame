using Assets.Scripts.Network.Resources;
using Assets.Scripts.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class Login : MonoBehaviour
{

    /// <summary>
    /// Submit button Reference
    /// </summary>
    public Button submitBtn;

    /// <summary>
    /// Remmeber input data on success login object
    /// </summary>
    public Toggle remmeber;

    /// <summary>
    /// UI Gameobject Reference
    /// </summary>
    private GameObject UI;

    /// <summary>
    /// Login form inputs
    /// </summary>
    public InputField usrField, pwdField;

    /// <summary>
    /// Login Panel ref
    /// </summary>
    public GameObject LoginPanel;


    /// <summary>
    /// Control var for submit btn
    /// </summary>
    public bool canSubmit = false;

    /// <summary>
    /// Verifies if we have connection before try something
    /// </summary>
    protected void Awake()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            Debug.Log("Error. Check internet connection!");
            var modal = new Modal("No internet connectivity", "Please verify your connection with the internet", Modal.Type.Information);
            modal.Render();

        }
    }

    /// <summary>
    /// Initializes objects and binds
    /// </summary>
    protected void Start()
    {
        //Reference validation
        if (UI == null) UI = GameObject.FindWithTag("UI").gameObject;
        if (LoginPanel == null) Debug.LogError("LoginPanel is missing from login.cs");
        if (remmeber == null) Debug.LogError("remmeber is missing from login.cs");
        if (submitBtn == null) Debug.LogError("submitBtn is missing from login.cs");
        if (usrField == null) Debug.LogError("usrField is missing from login.cs");
        if (pwdField == null) Debug.LogError("pwdField is missing from login.cs");

        //Unity Binds
        submitBtn.onClick.AddListener(SubmitValidation);
        remmeber.onValueChanged.AddListener(delegate { OnRemmeberChange(remmeber); });
        usrField.onValueChanged.AddListener(delegate { OnUsernameChange(usrField); });
        pwdField.onValueChanged.AddListener(delegate { OnPasswordChange(pwdField); });

        //Start the network area
        SocketController.Initialize();
        SocketController.BindEvent(NetworkInstructions.Disconnect, OnServerOffline);
        SocketController.BindEvent(NetworkInstructions.Connect, OnConnectionEstablished);
        SocketController.BindEvent(NetworkInstructions.ServerOffline, OnServerOffline);

        //Hide login panel till we get a connection with the remote server
        HideLoginPanel();

        //Disable submit until we get data
        submitBtn.interactable = false;

        //Lets see if the user has saved or asked to
        if (PlayerPrefs.GetInt("login_remmeber") == 1)
        {
            remmeber.isOn = true;

            //Remmeber me input load
            if (!String.IsNullOrWhiteSpace(PlayerPrefs.GetString("login_username")))
                usrField.text = PlayerPrefs.GetString("login_username");
            else if (!String.IsNullOrWhiteSpace(PlayerPrefs.GetString("login_password")))
                pwdField.text = PlayerPrefs.GetString("login_password");

        }
    }

    /// <summary>
    /// Triggered when we connect with the server
    /// </summary>
    private void OnConnectionEstablished(NetworkMessage netMsg)
    {
        Debug.Log("Connection established with the remote server");
        UnityMainThreadDispatcher.Instance().Enqueue(() => ShowLoginPanel());
    }

    /// <summary>
    /// Hides the login panel
    /// </summary>
    private void HideLoginPanel()
    {
        LoginPanel.SetActive(false);

        var box = Instantiate(Resources.Load<GameObject>("Prefabs/Modals/LoadingBox"), UI.transform) as GameObject;
        box.name = "LoadingBox";
        box.transform.Find("Text").GetComponent<Text>().text = "Communicating with the server...";

    }

    /// <summary>
    /// Enables the login panel and binds
    /// </summary>
    private void ShowLoginPanel()
    {
        //Show the UI
        LoginPanel.SetActive(true);

        //Bind login opcodes events
        SocketController.BindEvent(NetworkInstructions.LOGIN_FAIL, OnLoginFail);
        SocketController.BindEvent(NetworkInstructions.LOGIN_SUCCESS, OnLoginSuccess);

        //Destroy the loading box
        Destroy(GameObject.Find("LoadingBox"));

    }

    /// <summary>
    /// Event for return key or submit button click
    /// </summary>
    public void SubmitValidation()
    {
        if (SocketController.IsConnected())
        {
            var p = new AuthData
            {
                username = usrField.text,
                password = pwdField.text
            };

            SocketController.Send(NetworkInstructions.LOGIN_REQUEST, p);

            LoginPanel.SetActive(false);

            var spin = Instantiate(Resources.Load<GameObject>("Prefabs/Modals/LoadingBox"), UI.transform) as GameObject;
            spin.name = "LoadingBox";
            spin.transform.Find("Text").GetComponent<Text>().text = "Authenticating..";
        }
        else
        {
            Debug.Log("Socket Controller has no instance or is not connected");
        }

    }

    /// <summary>
    /// Called when the server returns Login Fail opcode
    /// </summary>
    /// <param name="netMsg"></param>
    protected void OnLoginFail(NetworkMessage netMsg)
    {
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            var modal = new Modal("Login Fail", netMsg.ReadMessage<ActionError>().message, Modal.Type.Information);
            modal.AddCloseListener(() => {
                var listOfModals = GameObject.FindGameObjectsWithTag("Modal");
                if (listOfModals.Length > 0)
                {
                    for (int i = 0; i < listOfModals.Length; i++)
                        Destroy(listOfModals[i]);
                }
                //Destroy(modal.GetReference());

           });
            modal.Render();

            ShowLoginPanel();
        });
    }

    /// <summary>
    /// Called when the server returns Login Success opcode
    /// </summary>
    /// <param name="netMsg"></param>
    protected void OnLoginSuccess(NetworkMessage netMsg)
    {
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            LoginPanel.SetActive(false);
            if (GameObject.Find("LoadingBox") != null)
                Destroy(GameObject.Find("LoadingBox"));

            //Create a spinner
            var spin = Instantiate(Resources.Load<GameObject>("Prefabs/Modals/LoadingBox"), UI.transform) as GameObject;
            spin.name = "LoadingBox";
            spin.transform.Find("Text").GetComponent<Text>().text = "Loading Profile..";

            //Read profileData
            var profileData = netMsg.ReadMessage<ProfileData>();


            //We make observer for WhenData arrives in order to define dynamiclly when loading or loaded is done
            spin.transform.Find("Text").GetComponent<Text>().text = "Loading Cards..";
            SocketController.Send(NetworkInstructions.PLAYER_CARDS_REQUEST, null);

            //Save the username and pwd if the remmeber is set true
            if (PlayerPrefs.GetInt("login_remmeber") == 1)
            {
                PlayerPrefs.SetString("login_username", usrField.text);
                PlayerPrefs.SetString("login_password", pwdField.text);
                Debug.Log("Remmeber saved");
            }

            //Unbind the login handlers since we're going to load the next scene
            SocketController.UnbindEvent(NetworkInstructions.LOGIN_FAIL);
            SocketController.UnbindEvent(NetworkInstructions.LOGIN_REQUEST);
            SocketController.UnbindEvent(NetworkInstructions.LOGIN_SUCCESS);

            //Wipe notifier list if it gives problems trying to notify here
            SocketController.UnbindEvent(NetworkInstructions.ServerOffline);

            //Load next scene
            SceneManager.LoadSceneAsync("MainMenu");
        });
    }

    /// <summary>
    /// Called when the user press to logout
    /// todo: Move from here because login does not have this option only in the next scenes
    /// </summary>
    protected void Logout()
    {
        if (SocketController.IsConnected())
        {
            Debug.Log("Requesting to logout..");
            SocketController.Send(NetworkInstructions.LOGOUT_REQUEST, null);
        }
    }

    /// <summary>
    /// When toogle changes we save it
    /// </summary>
    /// <param name="_toggle"></param>
    private void OnRemmeberChange(Toggle _toggle)
    {
        if (_toggle.isOn)
        {
            PlayerPrefs.SetInt("login_remmeber", 1);
            //It's just set the usr and pwd when login is succesfull
        }
        else
        {
            PlayerPrefs.SetInt("login_remmeber", 0);
            PlayerPrefs.SetString("login_username", "");
            PlayerPrefs.SetString("login_password", "");
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="input"></param>
    private void OnUsernameChange(InputField input)
    {
        if (String.IsNullOrWhiteSpace(input.text))
            submitBtn.interactable = false;
        else if (String.IsNullOrWhiteSpace(pwdField.text))
            submitBtn.interactable = false;
        else
        {
            submitBtn.interactable = true;
            canSubmit = true;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="input"></param>
    private void OnPasswordChange(InputField input)
    {
        if (String.IsNullOrWhiteSpace(input.text))
            submitBtn.interactable = false;
        else if (String.IsNullOrWhiteSpace(usrField.text))
            submitBtn.interactable = false;
        else
        {
            submitBtn.interactable = true;
            canSubmit = true;
        }

    }

    /// <summary>
    /// When the server is offline or we can't reach him just quit and notify the player
    /// </summary>
    private void OnServerOffline(NetworkMessage netMsg)
    {
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            Debug.Log("If we got disconnect here means the server is offline because we have no session ");
            var m = new Modal("Server is offline", "Servers are under maintenance or offline", Modal.Type.Information);
            m.AddCloseListener(delegate
            {
                Destroy(m.GetReference());
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
            });

            m.Render();
        });
    }

    /// <summary>
    /// TODO Soon
    /// </summary>
    protected void OnApplicationQuit()
    {
        //Shutdown the socket
        if (SocketController.GetSocketClient() != null && SocketController.GetSocketClient().Client.Connected)
        {
            Debug.Log("Requesting the socket to get lose");
            SocketController.GetSocketClient().Disconnect();
        }
    }







}
