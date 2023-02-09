using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class CardItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private int index;

    [SerializeField]
    private float pointerEnterSpacing;

    private RectTransform rightCard,
        leftCard;

    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.DOScale(2.5f, 0.25f);
        index = transform.GetSiblingIndex();
        transform.SetAsLastSibling();
        /* for (int i = index + 1; i < transform.parent.childCount; i++)
         {
             rightCard = transform.parent.GetChild(i).GetComponent<RectTransform>();
             rightCard.DOAnchorPosX(rightCard.anchoredPosition.x + pointerEnterSpacing, 0.25f);
         }
         for (int i = index - 1; i >= 0; i--)
         {
             leftCard = transform.parent.GetChild(index - i).GetComponent<RectTransform>();
             leftCard.DOAnchorPosX(leftCard.anchoredPosition.x - pointerEnterSpacing, 0.25f);
         }*/
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.DOScale(1.5f, 0.25f);
        transform.SetSiblingIndex(index);
        /* for (int i = index + 1; i < transform.parent.childCount; i++)
         {
             rightCard = transform.parent.GetChild(i).GetComponent<RectTransform>();
             rightCard.DOAnchorPosX(rightCard.anchoredPosition.x - pointerEnterSpacing, 0.25f);
         }
         for (int i = index - 1; i >= 0; i--)
         {
             leftCard = transform.parent.GetChild(index - i).GetComponent<RectTransform>();
             leftCard.DOAnchorPosX(leftCard.anchoredPosition.x + pointerEnterSpacing, 0.25f);
         }*/
    }
}
