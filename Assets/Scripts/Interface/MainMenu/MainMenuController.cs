using Assets.Scripts.Network.Resources;
using Assets.Scripts.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    public Button SocialBtn, QuitBtn, SettingsBtn;

    /// <summary>
    /// Soon refactor to a prefab because we will instantiate instead of hide and show
    /// </summary>
    public GameObject SettingsPanel, SocialPanel;


    public Text playerName, playerLevel, playerShards, playerGold;

    protected void Awake()
    {
        //Bind it
        SocketController.BindEvent(MsgType.Disconnect, OnDisconnect);

 
        //Load the resources here but soon move it to the loading thing
        //GameData.Initialize();
    }


    private void OnDisconnect(NetworkMessage netMsg)
    {
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {

            var m = new Modal("Disconnect", "Disconnected from the server", Modal.Type.Information);
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

    protected void Start()
    {
        if (SocialBtn == null) Debug.LogError("Please add a Social Btn in main menu controller");
        if (QuitBtn == null) Debug.LogError("Please add a QuitBtn in main menu controller");
        if (SettingsBtn == null) Debug.LogError("Please add a SettingsBtn in main menu controller");
        if (SocialPanel == null) Debug.LogError("SocialPanel is missing in main menu controller");


        //Refactor this soon, just make as shortcut for now
        if (SettingsBtn != null)
            SettingsBtn.onClick.AddListener(delegate
            {
                SettingsPanel.SetActive(true);
                SocialBtn.GetComponent<AudioSource>().Play();
            });
        if (QuitBtn != null)
            QuitBtn.onClick.AddListener(delegate
            {
                QuitBtn.GetComponent<AudioSource>().Play();
                //Instantiate the modal
                var modal = new Modal("QUIT", "Do you want to quit?", Modal.Type.QuitGame);
                modal.AddConfirmListener(() => Application.Quit());
                modal.AddCancelListener(() => Destroy(modal.GetReference()));
                modal.Render();
            });

        if (SocialBtn != null)
            SocialBtn.onClick.AddListener(delegate
            {
                if (!SocialPanel.activeSelf)
                    SocialPanel.SetActive(true);

                SocialBtn.GetComponent<AudioSource>().Play();
                if (!SocialPanel.GetComponent<UI_Social>().isActive)
                {
                    SocialPanel.GetComponent<Animator>().SetTrigger("SlideIn");
                    SocialPanel.GetComponent<UI_Social>().isActive = true;
                }
                else
                {
                    SocialPanel.GetComponent<UI_Social>().isActive = false;
                    SocialPanel.GetComponent<Animator>().SetTrigger("SlideOut");
                }
            });
    }

    /// <summary>
    /// We have to disconnect manually the socket otherwise it will be forever running into Unity Thread's
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
