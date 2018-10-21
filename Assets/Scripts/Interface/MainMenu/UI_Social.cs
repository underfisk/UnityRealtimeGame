using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
public class UI_Social : MonoBehaviour
{
    public Button CloseSocial;

    [Header("Tab buttons handlers")]
    public Button QuestsBtn;
    public Button FriendsBtn;
    public Button NotificationsBtn;

    [Header("Tabs")]
    public GameObject QuestBoard;
    public GameObject FriendsBoard;
    public GameObject NotificationBoard;

    private Animator _animator;
    public bool isActive = false;

    public enum Tab
    {
        Quests,
        Friends,
        Notifications
    }

    protected void Start()
    {
        if (CloseSocial == null) Debug.LogError("CloseSocial is missing at UI_Social");
        if (QuestsBtn == null) Debug.LogError("QuestsBtn is missing at UI_Social");
        if (_animator == null) _animator = gameObject.GetComponent<Animator>();

        if (gameObject.activeSelf)
            isActive = true;


        if (CloseSocial != null)
            CloseSocial.onClick.AddListener(delegate
            {
                _animator.SetTrigger("SlideOut");
                isActive = false;
            });

        //Bind Events
        if (QuestsBtn != null)
            QuestsBtn.onClick.AddListener(delegate
            {
                SetButtonActive(Tab.Quests);
                SetPanelActive(Tab.Quests);
            });
        if (FriendsBtn != null)
            FriendsBtn.onClick.AddListener(delegate 
            {
                SetButtonActive(Tab.Friends);
                SetPanelActive(Tab.Friends);
            });
        if (NotificationsBtn != null)
            NotificationsBtn.onClick.AddListener(delegate
            {
                SetButtonActive(Tab.Notifications);
                SetPanelActive(Tab.Notifications);
            });
        //Default is quests
        SetButtonActive(Tab.Quests);

    }

    /// <summary>
    /// Defines the active button layer
    /// </summary>
    /// <param name="t"></param>
    private void SetButtonActive(Tab t)
    {
        switch(t)
        {
            case Tab.Quests:
                QuestsBtn.transform.Find("Active").gameObject.SetActive(true);
                FriendsBtn.transform.Find("Active").gameObject.SetActive(false);
                NotificationsBtn.transform.Find("Active").gameObject.SetActive(false);
                break;
            case Tab.Friends:
                QuestsBtn.transform.Find("Active").gameObject.SetActive(false);
                FriendsBtn.transform.Find("Active").gameObject.SetActive(true);
                NotificationsBtn.transform.Find("Active").gameObject.SetActive(false);
                break;
            case Tab.Notifications:
                QuestsBtn.transform.Find("Active").gameObject.SetActive(false);
                FriendsBtn.transform.Find("Active").gameObject.SetActive(false);
                NotificationsBtn.transform.Find("Active").gameObject.SetActive(true);
                break;
        }
    }

    /// <summary>
    /// Sets the panel active 
    /// </summary>
    /// <param name="t"></param>
    private void SetPanelActive(Tab t)
    {
        switch (t)
        {
            case Tab.Quests:
                QuestBoard.SetActive(true);
                FriendsBoard.SetActive(false);
                NotificationBoard.SetActive(false);
                break;
            case Tab.Friends:
                QuestBoard.SetActive(false);
                FriendsBoard.SetActive(true);
                NotificationBoard.SetActive(false);
                break;
            case Tab.Notifications:
                QuestBoard.SetActive(false);
                FriendsBoard.SetActive(false);
                NotificationBoard.SetActive(true);
                break;
        }
    }


}
