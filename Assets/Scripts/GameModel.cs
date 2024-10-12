using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;


public class GameModel
{
    private List<RowData> _rows = new List<RowData>();
    private List<CardData> _deckCards = new List<CardData>();
    private SolitaireData _data;


    public event Action<CardData, RowData>         OnCardAddedToRow = delegate {};
    public event Action<CardData, RowData>         OnCardOpened     = delegate {};
    public event Action<CardData, RowData>         OnCardRemoved    = delegate {};
    public event Action<StackOfCardsData, RowData> OnStackAdded     = delegate {};
    public event Action<StackOfCardsData, RowData> OnStackRemoved     = delegate {};

    public GameModel(SolitaireData data)
    {
        _data = data;

        Initialize();
    }

    private void Initialize()
    {
        InitRows();
        InitDeckCards();
    }

    private void InitRows()
    {
        for ( int i = 0; i < _data.rowCount; i++ )
        {
            RowData row = new RowData(i);
            row.OnStackAdded  += OnStackAddedToRow;
            row.OnCardRemoved += OnCardRemovedFromRow;
            row.OnStackRemoved += OnStackRemovedFromRow;
            row.OnCardOpened += OnCardOpenedInRow;

            _rows.Add(row);
        }
    }

    private void OnCardOpenedInRow(CardData card, RowData row)
    {
        OnCardOpened(card, row);
    }

    private void OnStackRemovedFromRow( StackOfCardsData stack, RowData row )
    {
        OnStackRemoved(stack, row);
        row.OpenLastCard();
    }

    private void OnStackAddedToRow( StackOfCardsData arg1, RowData arg2 )
    {
        OnStackAdded(arg1, arg2);
    }

    private void InitDeckCards()
    {
        int cardId = 0;
        for ( int i = 0; i < _data.setCount; i++ )
        {
            for ( CardType cardType = CardType.Ace; cardType <= CardType.King; cardType++ )
            {
                _deckCards.Add( new CardData( cardId++, cardType, CardColor.Black ) );
            }
        }

        Random rng = new Random();
        _deckCards = _deckCards.OrderBy(_ => rng.Next()).ToList();
    }

    private void OnCardRemovedFromRow( CardData card, RowData row )
    {
        OnCardRemoved(card, row);
        row.OpenLastCard();
    }

    public void StartSetup()
    {
        AddCardsFromDeckToRows(_data.startCardsCount);
        OpenLastCardInRows();
    }

    public void AddCardsFromDeckToRows(int count)
    {
        List<CardData> cards = _deckCards.Take(count).ToList();

        for ( int i = 0; i < cards.Count; i++ )
        {
            int rowIndex = (int)Mathf.Repeat( i, _rows.Count );
            _rows[rowIndex].AddCard( cards[i] );

            OnCardAddedToRow(cards[i], _rows[rowIndex]);
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

    public int GetRowCount()
    {
        return _rows.Count;
    }

    public bool CanTakeStackOfCards(int cardId, int rowId)
    {
        return _rows[rowId].CanTakeStack(cardId);
    }

    public StackOfCardsData TakeStackOfCards(int cardId, int rowId)
    {
        return _rows[rowId].TakeStack( cardId );
    }

    public bool CanAddStackOfCards(StackOfCardsData stackOfCardsData, int rowId)
    {
        if (stackOfCardsData == null)
            return false;

        return _rows[rowId].CanAddStack(stackOfCardsData);
    }

    public void AddStackOfCards(StackOfCardsData stackOfCardsData, int rowId)
    {
        _rows[rowId].AddStack(stackOfCardsData);
    }

    public void RemoveStackOfCards(StackOfCardsData stack, int rowId)
    {
        _rows[rowId].RemoveStack(stack);
    }
}
