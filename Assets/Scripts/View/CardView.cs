using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class CardView : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private Image _cardImage;
    [SerializeField] private List<Sprite> _cardSprites;
    [SerializeField] private Sprite _closedSprite;

    private CardData _card;
    private Sequence _sequence;

    public int Id => _card.id;

    public event Action<CardView> OnClickedCard = delegate {};


    public void Init(CardData cardData)
    {
        _card             = cardData;
        _cardImage.sprite = _closedSprite;

        if (_card.isOpen)
        {
            SetCardImage( _card.type );
        }
    }

    public void Open()
    {
        if (_sequence != null)
            return;

        _sequence = DOTween.Sequence();
        _sequence.Append( transform.DOScaleX( 0.0f, 0.1f ) );
        _sequence.AppendCallback( () => SetCardImage( _card.type ) );
        _sequence.Append( transform.DOScaleX( 1.0f, 0.1f ) );
        _sequence.Play();
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
}
