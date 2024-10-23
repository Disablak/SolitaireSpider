using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class RowView : MonoBehaviour, IPointerEnterHandler
{
    [SerializeField] public Transform _cardContainer;

    private int             _id    = -1;
    private List<CardView>  _cards = new();
    private CardViewFactory _cardFactory;

    public List<CardView> Cards => _cards;
    public int            Id    => _id;

    public event Action<CardView, RowView> OnClickedCardInRow  = delegate {};
    public event Action<int>               OnPointerEnterInRow = delegate {};


    public void Init(int id, CardViewFactory cardFactory)
    {
        _id          = id;
        _cardFactory = cardFactory;
    }

    public void AddCard(CardView card)
    {
        card.transform.SetParent(_cardContainer);
        card.OnClickedCard += OnClickedCard;

        RectTransform layoutGroup = _cardContainer.GetComponent<RectTransform>();
        LayoutRebuilder.ForceRebuildLayoutImmediate(layoutGroup);

        _cards.Add(card);

        TryToShowBlockedCards();
    }

    public void RemoveStack(StackOfCardsData stack)
    {
        List<CardView> cardsToRemove = GetCardsByStack(stack);

        foreach (CardView cardView in cardsToRemove)
            _cardFactory.PoolCard(cardView);

        _cards = _cards.Except(cardsToRemove).ToList();
    }

    public void OpenCard(CardData cardData)
    {
        CardView cardView = _cards.FirstOrDefault(x => x.Id == cardData.id);
        if (cardView)
            cardView.Open();

        TryToShowBlockedCards();
    }

    private void TryToShowBlockedCards()
    {
        if (_cards.Count == 0)
            return;

        List<CardView> reverseCards = new(_cards);
        reverseCards.Reverse();

        CardType checkCardType = reverseCards[0].Type;
        bool     blockAll      = false;

        for (int i = 0; i < reverseCards.Count; i++)
        {
            CardView cardView = reverseCards[i];
            if (cardView.Type == checkCardType)
            {
                cardView.BlockOrUnblock(false);

                if (checkCardType < CardType.King)
                    checkCardType++;
            }
            else
                blockAll = true;

            if (blockAll && cardView.IsOpen) cardView.BlockOrUnblock(true);
        }
    }

    public void ShowOrHideStack(StackOfCardsData stack, bool show)
    {
        List<CardView> cardsToRemove = GetCardsByStack(stack);

        foreach (CardView cardView in cardsToRemove) cardView.gameObject.SetActive(show);
    }

    public void ShowOrHideCardSprite(int id, bool show)
    {
        CardView cardView = _cards.FirstOrDefault(x => x.Id == id);
        if (cardView)
            cardView.ShowOrHideSprite(show);
    }

    public CardView GetCardView(int id)
    {
        return _cards.FirstOrDefault(x => x.Id == id);
    }

    public List<CardView> GetCardsByStack(StackOfCardsData stack)
    {
        List<CardView> cards = new();
        for (int i = 0; i < Cards.Count; i++)
        {
            CardView cardView = Cards[i];
            if (stack.Cards.Select(x => x.id).Contains(cardView.Id)) cards.Add(cardView);
        }

        return cards;
    }

    private void OnClickedCard(CardView cardView)
    {
        OnClickedCardInRow(cardView, this);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        OnPointerEnterInRow(_id);
    }
}