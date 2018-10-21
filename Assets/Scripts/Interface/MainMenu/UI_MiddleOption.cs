using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
public class UI_MiddleOption : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private GameObject hoverLayer;
    private GameObject name, description;
    private Animator _animator;

    protected void Start()
    {
        if (hoverLayer == null)
            hoverLayer = gameObject.transform.Find("HoverGlow").gameObject;
        if (name == null)
            name = gameObject.transform.Find("Name").gameObject;
        if (description == null)
            description = gameObject.transform.Find("Description").gameObject;
        if (_animator == null)
            _animator = gameObject.GetComponent<Animator>();
    }

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        hoverLayer.SetActive(true);
        _animator.SetTrigger("Up");
    }

    
    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
        hoverLayer.SetActive(false);
        _animator.SetTrigger("Down");
    }
}
