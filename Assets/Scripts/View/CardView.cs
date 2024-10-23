using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class CardView : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private CanvasGroup  _canvasGroup;
    [SerializeField] private Image        _cardImage;
    [SerializeField] private Image        _cardBlockedImage;
    [SerializeField] private List<Sprite> _cardSprites;
    [SerializeField] private Sprite       _closedSprite;

    private CardData _card;
    private Sequence _sequence;
    private bool     _isOpen = false;

    public int      Id     => _card.id;
    public CardType Type   => _card.type;
    public bool     IsOpen => _card.isOpen;

    public event Action<CardView> OnClickedCard = delegate {};


    public void Init(CardData cardData)
    {
        _card             = cardData;
        _cardImage.sprite = _closedSprite;
        _isOpen           = cardData.isOpen;

        if (_isOpen)
            SetCardImage(_card.type);
    }

    public void Open()
    {
        if (_isOpen)
            return;

        PlayAnimationOpen();
    }

    private void PlayAnimationOpen()
    {
        _sequence = DOTween.Sequence();
        _sequence.Append(transform.DOScaleX(0.0f, 0.1f));
        _sequence.AppendCallback(() => SetCardImage(_card.type));
        _sequence.Append(transform.DOScaleX(1.0f, 0.1f));
        _sequence.AppendCallback(() => _isOpen = true);
        _sequence.Play();
    }

    public void BlockOrUnblock(bool block)
    {
        _cardBlockedImage.gameObject.SetActive(block);
    }

    public void DisableRaycastTarget()
    {
        _cardImage.raycastTarget = false;
    }

    public void ShowOrHide(bool show)
    {
        gameObject.SetActive(show);
    }

    public void ShowOrHideSprite(bool show)
    {
        _canvasGroup.alpha = show ? 1 : 0;
    }

    private void SetCardImage(CardType cardType)
    {
        _cardImage.sprite = _cardSprites[(int)cardType - 1];
    }

    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
        OnClickedCard(this);
    }
}