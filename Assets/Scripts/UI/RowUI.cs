using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;


public class RowUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] public Transform _cardContainer;

    private int _id = -1;
    private List<CardView> _cards = new List<CardView>();

    public List<CardView> Cards => _cards;
    public int Id => _id;

    public event Action<CardView, RowUI> OnClickedCardInRow = delegate{};
    public event Action<int> OnPointerEnterInRow = delegate{};


    public void SetId(int id)
    {
        _id = id;
    }

    public void AddCard(CardView card)
    {
        card.transform.SetParent(_cardContainer);

        card.OnClickedCard += OnClickedCard;
        _cards.Add( card );
    }

    public void RemoveStack(StackOfCardsData stack)
    {
        List<CardView> cardsToRemove = new List<CardView>();
        for ( int i = 0; i < Cards.Count; i++ )
        {
            var cardView = Cards[i];
            if (stack.Cards.Select( x => x.id ).Contains( cardView.Id ))
            {
                cardsToRemove.Add( cardView );
            }
        }

        foreach ( CardView cardView in cardsToRemove )
        {
            Destroy(cardView.gameObject);
        }

        _cards = _cards.Except( cardsToRemove ).ToList();
    }

    public void OpenCard(CardData cardData)
    {
        CardView cardView = _cards.FirstOrDefault(x => x.Id == cardData.id);
        if (cardView)
            cardView.Open();
    }

    private void OnClickedCard(CardView cardView)
    {
        OnClickedCardInRow(cardView, this);
    }

    public void OnPointerEnter( PointerEventData eventData )
    {
        OnPointerEnterInRow(_id);
    }

    public void OnPointerExit( PointerEventData eventData )
    {
        //Debug.Log( $"exit {_id}" );
    }
}
