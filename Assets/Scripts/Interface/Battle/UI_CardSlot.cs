using Assets.Scripts.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_CardSlot : MonoBehaviour
{
    public Card CardData { get; set; }

    public bool IsEmpty
    {
        get { return CardData != null; }
        private set { }
    }

    protected void Start()
    {
        
    }


}
