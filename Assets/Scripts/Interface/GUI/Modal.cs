using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    /// <summary>
    /// This class stands for rendering a modal box in UI
    /// </summary>
    public class Modal 
    {
        /// <summary>
        /// Enumeration of Modal Types
        /// </summary>
        public enum Type
        {
            Loading,
            Close,
            Information,
            Warning,
            Error,
            Question,
            Notification,
            Disconnect,
            QuitGame
            //.. buying an item etc, playing card, etc, overtime..
        }

        /// <summary>
        /// Holds the modal prefabs path to be loaded
        /// </summary>
        private String modal_info_path = "Prefabs/Modals/Modal_Info",
            modal_warn_path = "Prefabs/Modals/Modal_Warning",
            modal_quit_path = "Prefabs/Modals/Modal_QuitGame",
            modal_error_path = "Prefabs/Modals/Modal_Error";

        /// <summary>
        /// Instance for UI Gameobject
        /// </summary>
        private GameObject UI;

        /// <summary>
        /// Holds the title and the body message
        /// </summary>
        private String _title, _msg;

        /// <summary>
        /// Saves the modal type
        /// </summary>
        private Type _type;

        /// <summary>
        /// Holds the reference to the instantiated modal 
        /// </summary>
        private GameObject modal_ref = null;

        /// <summary>
        /// Flag to know whether this modal has been already instantiated
        /// </summary>
        private bool instantiated = false;

        /// <summary>
        /// Delegates an OnButtonAction event type
        /// </summary>
        public delegate void OnButtonAction();

        /// <summary>
        /// Event observer containing OnButtonAction delegates
        /// </summary>
        private event OnButtonAction OnCloseObservers,
                                    OnConfirmObservers, 
                                    OnCancelObservers;

        /// <summary>
        /// Verifies when he scripts awakes if we have everything set
        /// </summary>
        public Modal(String title, String msg, Type type)
        {
            this._title = title;
            this._msg = msg;
            this._type = type;

            if (UI == null) UI = GameObject.FindWithTag("UI").gameObject;
        }

        /// <summary>
        /// Renders a modal window filtered by Type
        /// </summary>
        /// <param name="title"></param>
        /// <param name="msg"></param>
        /// <param name="type"></param>
        internal void Render()
        {
            if (!this.instantiated)
            {
                switch (_type)
                {
                    case Type.Information:
                        //Get the info window
                        var prefabInfo = GameObject.Instantiate(Resources.Load(this.modal_info_path) as GameObject, UI.transform) as GameObject;
                        prefabInfo.name = "Modal_Info";
                        prefabInfo.transform.Find("Title").gameObject.GetComponent<Text>().text = this._title;
                        prefabInfo.transform.Find("Message").gameObject.GetComponent<Text>().text = this._msg;
                        prefabInfo.transform.Find("CloseModal").gameObject.GetComponent<Button>().onClick.AddListener(OnCloseAction);
                        //Saves ref
                        this.modal_ref = prefabInfo.gameObject;
                        this.instantiated = true;
                        break;

                    case Type.Error:
                        //Get the info window
                        var prefabError = GameObject.Instantiate(Resources.Load(this.modal_error_path) as GameObject, UI.transform) as GameObject;
                        prefabError.name = "Modal_Error";
                        prefabError.transform.Find("Title").gameObject.GetComponent<Text>().text = this._title;
                        prefabError.transform.Find("Message").gameObject.GetComponent<Text>().text = this._msg;
                        prefabError.transform.Find("CloseModal").gameObject.GetComponent<Button>().onClick.AddListener(OnCloseAction);
                        //Saves ref
                        this.modal_ref = prefabError.gameObject;
                        this.instantiated = true;
                        break;
                    case Type.Warning:
                        break;
                    case Type.Notification:
                        break;
                    case Type.Question:
                        break;
                    case Type.QuitGame:
                        var prefabQuit = GameObject.Instantiate(Resources.Load(this.modal_quit_path) as GameObject, UI.transform) as GameObject;
                        prefabQuit.name = "Modal_QuitGame";
                        prefabQuit.transform.Find("Title").gameObject.GetComponent<Text>().text = this._title;
                        prefabQuit.transform.Find("Message").gameObject.GetComponent<Text>().text = this._msg;
                        prefabQuit.transform.Find("ConfirmModal").gameObject.GetComponent<Button>().onClick.AddListener(OnConfirmAction);
                        prefabQuit.transform.Find("CancelModal").gameObject.GetComponent<Button>().onClick.AddListener(OnCancelAction);

                        //Saves ref
                        this.modal_ref = prefabQuit.gameObject;
                        this.instantiated = true;
                        break;
                   

                }
            }
            else
                Debug.LogWarning("You are trying to instantiate an modal that was been already instantiated");
        }

        /// <summary>
        /// Returns the instantiated prefab reference or null
        /// Make sure just to call this function after Render function
        /// </summary>
        /// <returns></returns>
        public GameObject GetReference()
        {
            return this.modal_ref ?? null;
        }

        /// <summary>
        /// When the close action is pressed this function is called to notify the observers of this change
        /// </summary>
        private void OnCloseAction()
        {
            if (OnCloseObservers != null)
                OnCloseObservers.Invoke();
        }

        /// <summary>
        /// When the confirm action is pressed this function is called to notify the observers of this change
        /// </summary>
        private void OnConfirmAction()
        {
            if (OnConfirmObservers != null)
                OnConfirmObservers.Invoke();
        }

        /// <summary>
        /// When the cancel action is pressed this function is called to notify the observers of this change
        /// </summary>
        private void OnCancelAction()
        {
            if (OnCancelObservers != null)
                OnCancelObservers.Invoke();
        }

        /// <summary>
        /// Binds a OnButtonAction delegate to handle a close event
        /// </summary>
        /// <param name="handler"></param>
        public void AddCloseListener(OnButtonAction handler)
        {
            OnCloseObservers += handler;
        }


        /// <summary>
        /// Binds OnButtonAction delegate to handle confirm event
        /// </summary>
        /// <param name="handler"></param>
        public void AddConfirmListener(OnButtonAction handler)
        {
            OnConfirmObservers += handler;
        }

        /// <summary>
        /// Binds OnButtonAction delegate to handle cancel event
        /// </summary>
        /// <param name="handler"></param>
        public void AddCancelListener(OnButtonAction handler)
        {
            OnCancelObservers += handler;
        }
    }
}
