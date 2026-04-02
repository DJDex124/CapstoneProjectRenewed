using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using NUnit.Framework.Interfaces;

public class OldItemSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public bool selected;
    public OldItemData itemInSlot;
    public int itemCount;
    public int index;


    [HideInInspector] public Image slotImage;
    [HideInInspector] public Image SpriteImage;
    [HideInInspector] public TextMeshProUGUI itemCountText;

    private void Start()
    {
        slotImage = GetComponent<Image>();
        SpriteImage = transform.GetChild(0).GetComponent<Image>();
        itemCountText = GetComponentInChildren<TextMeshProUGUI>();

        if (itemInSlot != null)
        {
            SpriteImage.enabled = false;
            itemCountText.enabled = false;
        }
    }
    public void Init()
    {
        slotImage = GetComponent<Image>();
        SpriteImage = transform.GetChild(0).GetComponent<Image>();
        itemCountText = GetComponentInChildren<TextMeshProUGUI>();

        SpriteImage.enabled = false;
        itemCountText.enabled = false;

        itemInSlot = null;
        itemCount = 0;
    }
    void Update()
    {
        if (selected)
        {
            slotImage.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
        }
        else
        {
            slotImage.transform.localScale = new Vector3(1f, 1f, 1f);
        }
    }


    public void OnPointerClick(PointerEventData eventData)
    {

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        selected = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        selected = false;
    }
}
