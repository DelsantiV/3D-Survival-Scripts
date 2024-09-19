using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class Dragable : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{

    protected RectTransform rectTransform;
    protected CanvasGroup canvasGroup;

    protected Vector3 startPosition;
    protected Transform startParent;

    public virtual void Initialize()
    {

        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
    }


    public virtual void OnBeginDrag(PointerEventData eventData)
    {

        Debug.Log("OnBeginDrag");
        //make it a bit transparent
        canvasGroup.alpha = .6f;
        //So the ray cast will ignore the item itself.
        canvasGroup.blocksRaycasts = false;
        startPosition = transform.position;
        startParent = transform.parent;
        transform.SetParent(transform.root);

    }

    public virtual void OnDrag(PointerEventData eventData)
    {
        //So the item will move with our mouse (at same speed)
        rectTransform.anchoredPosition += eventData.delta;
    }



    public virtual void OnEndDrag(PointerEventData eventData)
    {
        if (transform.parent == startParent || transform.parent == transform.root)
        {
            Debug.Log("Replacing item to start slot");
            
            transform.position = startPosition;
            transform.SetParent(startParent);
        }

        Debug.Log("OnEndDrag");
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
    }



}