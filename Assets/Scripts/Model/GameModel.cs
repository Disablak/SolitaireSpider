using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;


public class GameModel
{
    private List<RowData> _rows = new List<RowData>();
    private List<CardData> _deckCards = new List<CardData>();
    private GameData _data;

    public event Action<Dictionary<CardData, RowData>> OnCardsAddedToRows = delegate {};
    public event Action<CardData, RowData>             OnCardOpened       = delegate {};
    public event Action<StackOfCardsData, RowData>     OnStackAdded       = delegate {};
    public event Action<StackOfCardsData, RowData>     OnStackRemoved     = delegate {};
    public event Action<StackOfCardsData, RowData>     OnWinStackRemoved  = delegate {};
    public event Action<bool>                          OnGameOver         = delegate {};

    public GameModel(GameData data)
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
            row.OnStackRemoved += OnStackRemovedFromRow;
            row.OnWinStackRemoved += OnWinStackRemovedFromRow;
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

    private void OnWinStackRemovedFromRow( StackOfCardsData stack, RowData row )
    {
        OnWinStackRemoved(stack, row);

        row.OpenLastCard();
        CheckForWinGame();
    }

    public void StartSetup()
    {
        //Test0();
        //Test1();
        //Test2();

        AddCardsFromDeckToRows(_data.startCardsCount);
        OpenLastCardInRows();
    }

    public bool CanAddCards()
    {
        return _deckCards.Count > 0;
    }

    public void AddCardsFromDeckToRows()
    {
        AddCardsFromDeckToRows(_data.rowCount);
    }

    public void AddCardsFromDeckToRows(int count)
    {
        List<CardData> cards = _deckCards.Take(count).ToList();

        Dictionary<CardData, RowData> dicCardsToRows = new Dictionary<CardData, RowData>();

        for ( int i = 0; i < cards.Count; i++ )
        {
            int rowIndex = (int)Mathf.Repeat( i, _rows.Count );
            _rows[rowIndex].AddCard( cards[i] );

            dicCardsToRows.Add( cards[i], _rows[rowIndex] );
        }

        OnCardsAddedToRows(dicCardsToRows);

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

    public StackOfCardsData GetStackOfCards(int cardId, int rowId)
    {
        return _rows[rowId].GetStack( cardId );
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

    private void CheckForWinGame()
    {
        if (_deckCards.Count > 0)
            return;

        if (_rows.All( x => x.cards.Count == 0 ))
            OnGameOver?.Invoke( true );
    }

    #region Tests
    private void Test0()
    {
        _deckCards.Clear();

        var cards  = new List<CardData>();
        int cardId = 0;
        for ( CardType cardType = CardType.Ace; cardType <= CardType.King; cardType++ )
        {
            var card = new CardData( cardId++, cardType, CardColor.Black );
            card.Open();
            cards.Add( card );
        }

        cards.Reverse();

        var partOne = cards.Take( 6 ).ToList();
        var partTwo = cards.Skip( 6 ).ToList();

        Dictionary<CardData, RowData> dicCardsToRows = new Dictionary<CardData, RowData>();

        AddCardToRow( new CardData( cardId++, CardType.Five, CardColor.Black ), _rows[5] );

        foreach ( var card in partOne )
            AddCardToRow(card, _rows[5]);

        foreach ( var card in partTwo )
            AddCardToRow(card, _rows[6]);

        OnCardsAddedToRows(dicCardsToRows);

        void AddCardToRow(CardData card, RowData row)
        {
            row.AddCard( card );
            dicCardsToRows.Add( card, row );
        }
    }

    private void Test1()
    {
        _deckCards.Clear();

        var cards  = new List<CardData>();
        int cardId = 0;
        for ( CardType cardType = CardType.Ace; cardType <= CardType.King; cardType++ )
        {
            var card = new CardData( cardId++, cardType, CardColor.Black );
            card.Open();
            cards.Add( card );
        }

        cards.Reverse();

        var partOne = cards.Take( 6 ).ToList();
        var partTwo = cards.Skip( 6 ).ToList();

        Dictionary<CardData, RowData> dicCardsToRows = new Dictionary<CardData, RowData>();

        //AddCardToRow( new CardData( cardId++, CardType.Five, CardColor.Black ), _rows[5] );

        foreach ( var card in partOne )
            AddCardToRow(card, _rows[5]);

        foreach ( var card in partTwo )
            AddCardToRow(card, _rows[6]);






        cards.Clear();

        for ( CardType cardType = CardType.Ace; cardType <= CardType.King; cardType++ )
        {
            var card = new CardData( cardId++, cardType, CardColor.Black );
            card.Open();
            cards.Add( card );
        }

        cards.Reverse();

        var partThree = cards.Take( 6 ).ToList();
        var partFour = cards.Skip( 6 ).ToList();

        foreach ( var card in partThree )
            AddCardToRow(card, _rows[1]);

        foreach ( var card in partFour )
            AddCardToRow(card, _rows[2]);

        OnCardsAddedToRows(dicCardsToRows);



        void AddCardToRow(CardData card, RowData row)
        {
            row.AddCard( card );
            dicCardsToRows.Add( card, row );
        }
    }

    private void Test2()
    {
        _deckCards.Clear();

        var cards  = new List<CardData>();
        int cardId = 0;

        var card = new CardData( cardId++, CardType.Ace, CardColor.Black );
        card.Open();
        cards.Add( card );

        var card1 = new CardData( cardId++, CardType.Three, CardColor.Black );
        card1.Open();
        cards.Add( card1 );

        var card2 = new CardData( cardId++, CardType.Five, CardColor.Black );
        card2.Open();
        cards.Add( card2 );

        Dictionary<CardData, RowData> dicCardsToRows = new Dictionary<CardData, RowData>();

        foreach ( var cardData in cards )
            AddCardToRow(cardData, _rows[3]);

        OnCardsAddedToRows(dicCardsToRows);

        void AddCardToRow(CardData thiscard, RowData row)
        {
            row.AddCard( thiscard );
            dicCardsToRows.Add( thiscard, row );
        }
    }
    #endregion
}
