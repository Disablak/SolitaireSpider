using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameView : MonoBehaviour
{
    [SerializeField] private RowView _rowPrefab;
    [SerializeField] private CardView _cardPrefab;
    [SerializeField] private StackOfCardsView _stackOfCardsView;
    [SerializeField] private Transform _rowContainer;
    [SerializeField] private CardView _otherCard;
    [SerializeField] private PointerInput _pointerInput;
    [SerializeField] private CardViewTweens _cardViewTweens;


    private List<RowView> _rows = new List<RowView>();
    private GameModel _gameModel;
    public int _lastPointedRow = -1;
    private StackOfCardsData _currentStackOfCardsData;

    public void Init(GameModel gameModel) // TODO VIEW NOT ABLE TO SEE MODEL
    {
        _gameModel = gameModel;

        _pointerInput.OnMouseReleased += OnPointerReleased;

        _otherCard.OnClickedCard += OnClickOtherCard;
    }

    private void OnClickOtherCard(CardView obj)
    {
        if (_gameModel.CanAddCards())
        {
            _gameModel.AddCardsFromDeckToRows();
            _gameModel.OpenLastCardInRows();
        }
    }

    private void OnPointerReleased()
    {
        if (_lastPointedRow == -1)
        {
            return;
        }

        if (_gameModel.CanAddStackOfCards(_currentStackOfCardsData, _lastPointedRow))
        {
            _gameModel.AddStackOfCards(_currentStackOfCardsData, _lastPointedRow);
            _gameModel.RemoveStackOfCards(_currentStackOfCardsData, _currentStackOfCardsData.OriginRowId);
        }else
        {
            if (_pointerInput.IsStackStickedToPointer)
            {
                RowView rowView = _rows[_currentStackOfCardsData.OriginRowId];
                rowView.ShowOrHideStack(_currentStackOfCardsData, true);
            }
        }

        if (_pointerInput.IsStackStickedToPointer)
        {
            _stackOfCardsView.Deinit();
            _currentStackOfCardsData = null;
        }
    }

    public void AddRows(int count)
    {
        for ( int i = 0; i < count; i++ )
        {
            RowView row = Instantiate(_rowPrefab, _rowContainer);
            row.SetId(i);
            row.OnClickedCardInRow += OnClickedCardInRow;
            row.OnPointerEnterInRow += OnPointerEnterInRow;

            _rows.Add( row );
        }
    }

    private void OnPointerEnterInRow( int rowId )
    {
        _lastPointedRow = rowId;
    }

    private void OnClickedCardInRow(CardView cardView, RowView rowView)
    {
        if (!_gameModel.CanTakeStackOfCards( cardView.Id, rowView.Id ))
        {
            return;
        }

        StackOfCardsData stackOfCardsData = _gameModel.TakeStackOfCards(cardView.Id, rowView.Id);

        _stackOfCardsView.Init( stackOfCardsData );

        rowView.ShowOrHideStack(stackOfCardsData, false);

        _pointerInput.StickToPointer(_stackOfCardsView);
        _currentStackOfCardsData = stackOfCardsData;
    }

    public void AddCardToRow(CardData card, RowData row)
    {
        var rowView = _rows[row.id];
        var cardView = Instantiate( _cardPrefab, transform);

        cardView.Init(card);
        rowView.AddCard( cardView );
    }

    public void AddCardsToRows(Dictionary<CardData, RowData> dictionary)
    {
        Dictionary<CardView, RowView> dictionaryView = new Dictionary<CardView, RowView>();

        foreach ( KeyValuePair<CardData,RowData> value_pair in dictionary )
        {
            RowView rowView  = _rows[value_pair.Value.id];
            CardView cardView = Instantiate(_cardPrefab, transform);

            cardView.Init(value_pair.Key);
            dictionaryView.Add( cardView, rowView );
        }

        _cardViewTweens.TweenCardsToRows(dictionaryView);
    }

    public void OpenCard(CardData cardData, RowData rowData)
    {
        _rows[rowData.id].OpenCard( cardData );
    }

    public void AddStackOfCards(StackOfCardsData stackOfCardsData, RowData rowData)
    {
        foreach ( CardData card in stackOfCardsData.Cards )
        {
            AddCardToRow( card, rowData );
        }
    }

    public void RemoveStackOfCards(StackOfCardsData stackOfCardsData, RowData rowData)
    {
        _rows[rowData.id].RemoveStack(stackOfCardsData);
    }
}
