using System.Collections.Generic;
using UnityEngine;

public class StackOfCardsView : MonoBehaviour
{
    [SerializeField] CardView _cardPrefab;
    [SerializeField] Transform _cardsContainer;

    private List<CardView> _cards = new List<CardView>();


    public void Init(StackOfCardsData stackOfCardsData)
    {
        foreach ( CardData card in stackOfCardsData.Cards )
        {
            var cardView = Instantiate( _cardPrefab, _cardsContainer);
            cardView.Init(card);
        }
    }
}
