using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;


public class GameModel
{
    private List<RowData> _rows = new List<RowData>();
    private List<CardData> _deckCards = new List<CardData>();

    public GameModel(SolitaireData solitaireData)
    {
        for ( int i = 0; i < solitaireData.rowCount; i++ )
        {
            _rows.Add( new RowData() );
        }

        for ( int i = 0; i < solitaireData.setCount; i++ )
        {
            for ( CardType cardType = CardType.Ace; cardType <= CardType.King; cardType++ )
            {
                _deckCards.Add( new CardData( cardType, CardColor.Black ) );
            }
        }

        Random rng = new Random();
        _deckCards = _deckCards.OrderBy(_ => rng.Next()).ToList();

        AddCardsFromDeckToRows(solitaireData.startCardsCount);
        OpenLastCardInRows();

        _rows[0].cards.Clear();
        for ( CardType cardType = CardType.Ace; cardType <= CardType.King; cardType++ )
        {
            _rows[0].cards.Add( new CardData( cardType, CardColor.Black ) {isOpen = true} );
        }

        _rows[0].cards.Reverse();
        Debug.Log( _rows[0].IsValidSet() );
    }

    public void AddCardsFromDeckToRows(int count)
    {
        List<CardData> cards = _deckCards.Take(count).ToList();

        for ( int i = 0; i < cards.Count; i++ )
        {
            int rowIndex = (int)Mathf.Repeat( i, _rows.Count );
            _rows[rowIndex].AddCard( cards[i] );
        }

        _deckCards = _deckCards.Except( cards ).ToList();
    }

    public void OpenLastCardInRows()
    {
        foreach ( var row in _rows )
        {
            row.OpenLastCard();
        }
    }
}
