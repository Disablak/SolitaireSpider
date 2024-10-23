using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class StackOfCardsView : MonoBehaviour
{
    [SerializeField] private CardView  _cardPrefab;
    [SerializeField] private Transform _cardsContainer;

    private List<CardView> _cards = new();


    private void Awake()
    {
        _cards = _cardsContainer.GetComponentsInChildren<CardView>().ToList();

        foreach (CardView card in _cards)
            card.DisableRaycastTarget();
    }

    public void Init(StackOfCardsData stackOfCardsData)
    {
        gameObject.SetActive(true);

        foreach (CardView card in _cards)
            card.gameObject.SetActive(false);

        for (int i = 0; i < stackOfCardsData.Cards.Count; i++)
        {
            CardView cardView = _cards[i];
            CardData cardData = stackOfCardsData.Cards[i];
            cardView.Init(cardData);
            cardView.gameObject.SetActive(true);
        }
    }

    public void Deinit()
    {
        gameObject.SetActive(false);
    }
}