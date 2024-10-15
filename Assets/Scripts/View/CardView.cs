using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class CardView : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerMoveHandler
{
    [SerializeField] private Image _cardImage;
    [SerializeField] private List<Sprite> _cardSprites;
    [SerializeField] private Sprite _closedSprite;

    public int Id => _card.id;
    private CardData _card;

    public event Action<CardView> OnClickedCard = delegate {};


    public void Init(CardData cardData)
    {
        _card             = cardData;
        _cardImage.sprite = _closedSprite;

        if (_card.isOpen)
        {
            Open();
        }
    }

    public void Open()
    {
        SetCardImage( _card.type );
    }

    public void DisableRaycastTarget()
    {
        _cardImage.raycastTarget = false;
    }

    public void ShowOrHide(bool show)
    {
        gameObject.SetActive( show );
    }

    public void ShowOrHideSprite(bool show)
    {
        _cardImage.enabled = show;
    }

    private void SetCardImage(CardType cardType)
    {
        _cardImage.sprite = _cardSprites[(int)cardType - 1];
    }

    void IPointerDownHandler.OnPointerDown( PointerEventData eventData )
    {
        OnClickedCard(this);
    }

    void IPointerUpHandler.OnPointerUp( PointerEventData eventData )
    {
        //throw new System.NotImplementedException();
    }

    void IPointerMoveHandler.OnPointerMove( PointerEventData eventData )
    {
        //throw new System.NotImplementedException();
    }
}
