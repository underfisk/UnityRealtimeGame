using Assets.Scripts.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Card : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    //This vars have to be soon private 
    public Vector3 startPosition;

    public Transform rootParent;

    private GameObject UI;

    public GameObject hoverLayer, disabledLayer;

    private Card data;

    private float maxY, maxX;

    public Image idleShadow, hoverShadow;
    
    private bool previewingOnTable = false;

    //public Image targetImage;

    protected void Start()
    {
        if (UI == null) UI = GameObject.FindGameObjectWithTag("UI").gameObject;
        // if (targetImage == null) Debug.LogError("targetImage is missing in UI_card");
        if (idleShadow == null) Debug.LogError("Idle Shadow is missing on UI_Card");
        if (hoverShadow == null) Debug.LogError("Hover Shadow is missing on UI_Card");

        //test
        data = new Card(1,"hello");
    }


    /// <summary>
    /// Defines some important data also allows dragg
    /// </summary>
    /// <param name="eventData"></param>
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (data != null)
        {
            startPosition = gameObject.transform.localPosition;
            rootParent = gameObject.transform.parent.gameObject.transform;
            gameObject.transform.SetParent(UI.transform);
            GetComponent<CanvasGroup>().blocksRaycasts = false;

         //   Debug.Log("Dragging .."); 
        }
    }

    /// <summary>
    /// TODO: Fix the limit where we can drag
    /// </summary>
    /// <param name="eventData"></param>
    public void OnDrag(PointerEventData eventData)
    {
        if (data != null)
        {
            gameObject.transform.position = Input.mousePosition;
            //Debug.Log($"Mouse Position = [{Input.mousePosition.x}, {Input.mousePosition.y}]");
            var hitObject = eventData.pointerCurrentRaycast.gameObject;

            //Ok lets check free slot and show a prefab with some low alpha which is where our card can be placed
            if (hitObject != null && hitObject.name == "PlayerPlayArea")
            {
               if (!previewingOnTable)
                {
                    //Change the attributes
                    var cardPreview = Instantiate(gameObject) as GameObject;
                    cardPreview.transform.Find("Background").GetComponent<Image>().color = Color.grey;
                   
                    cardPreview.transform.SetParent(hitObject.transform);
                    cardPreview.transform.localScale = new Vector3(1f, 1f, 1f);
                    cardPreview.transform.localPosition = Vector3.zero;

                    cardPreview.name = "PreviewCard";
                    previewingOnTable = true;
                }
            }

        }
           
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        var targetObj = eventData.pointerCurrentRaycast.gameObject;

        //Do we have object of the type we want?
        //If doesn't work, refactor and see if is the play area and loop trought he's childs and check
        if (targetObj != null && targetObj.GetComponent<UI_CardSlot>())
        {
            var slot = targetObj.GetComponent<UI_CardSlot>();
            if (slot.IsEmpty)
            {
                Debug.Log("Ok the slot is empty we can move the card");
            }

        }

        //Remove this temporary Prefab
        previewingOnTable = false;
        if (GameObject.Find("PreviewCard"))
            Destroy(GameObject.Find("PreviewCard").gameObject);

        //Reset cuz in case we dont add the card
        gameObject.transform.position = startPosition;
        gameObject.transform.SetParent(rootParent);

        // Debug.Log("Dragged ended");
        GetComponent<CanvasGroup>().blocksRaycasts = true;

    }


    public void OnPointerEnter(PointerEventData edata)
    {
        //  Debug.Log("Something is hover me");

        //Highlight the border
        idleShadow.gameObject.SetActive(false);
        hoverShadow.gameObject.SetActive(true);
        
        //Show the card desc in right side or whatever

    }

    public void OnPointerExit(PointerEventData edata)
    {
        //Turn off the glow
        idleShadow.gameObject.SetActive(true);
        hoverShadow.gameObject.SetActive(false);
    }

}
