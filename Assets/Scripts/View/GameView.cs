using System.Collections.Generic;
using UnityEngine;


public class GameView : MonoBehaviour
{
    [SerializeField] private GamePresenter _presenter;
    [SerializeField] private RowView _rowPrefab;
    [SerializeField] private CardView _cardPrefab;
    [SerializeField] private StackOfCardsView _stackOfCardsView;
    [SerializeField] private Transform _rowContainer;
    [SerializeField] private CardView _otherCard;
    [SerializeField] private PointerInput _pointerInput;
    [SerializeField] private CardViewTweens _cardViewTweens;

    private List<RowView> _rows;
    private StackOfCardsData _currentStackOfCardsData;
    public int _lastPointedRow = -1;


    private void Awake()
    {
        _pointerInput.OnMouseReleased += OnPointerReleased;
        _otherCard.OnClickedCard += OnClickOtherCard;
    }

    private void OnClickOtherCard(CardView obj)
    {
        if (_cardViewTweens.IsCardsTweening)
            return;

        if (_presenter.CanAddNewCards())
            _presenter.AddNewCardsAndOpenLast();
    }

    private void OnPointerReleased()
    {
        if (_cardViewTweens.IsCardsTweening)
            return;

        if (_lastPointedRow == -1)
            return;

        if (_presenter.CanAddStackOfCards(_currentStackOfCardsData, _lastPointedRow))
            _presenter.MoveStackOfCards(_currentStackOfCardsData, _currentStackOfCardsData.OriginRowId, _lastPointedRow);
        else
            ReturnStackToPrev();

        HideStack();
    }

    private void ReturnStackToPrev()
    {
        RowView rowView = _rows[_currentStackOfCardsData.OriginRowId];
        rowView.ShowOrHideStack(_currentStackOfCardsData, true);
    }

    private void HideStack()
    {
        _stackOfCardsView.Deinit();
        _currentStackOfCardsData = null;
    }

    public void AddRows(int count)
    {
        _rows = new List<RowView>(count);

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
        if (_cardViewTweens.IsCardsTweening)
            return;

        if (!_presenter.CanTakeStackOfCards( cardView.Id, rowView.Id ))
            return;

        StackOfCardsData stackOfCardsData = _presenter.GetStackOfCards(cardView.Id, rowView.Id);

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
        Dictionary<CardData, RowView> dictionaryView = new Dictionary<CardData, RowView>();

        foreach ( KeyValuePair<CardData,RowData> value_pair in dictionary )
        {
            RowView rowView  = _rows[value_pair.Value.id];
            CardView cardView = Instantiate(_cardPrefab, transform);
            cardView.Init(value_pair.Key);
            cardView.ShowOrHideSprite( false );
            rowView.AddCard( cardView );

            dictionaryView.Add(value_pair.Key, rowView);
        }

        _cardViewTweens.TweenCardsToRows(dictionaryView);
    }

    public void OpenCard(CardData cardData, RowData rowData)
    {
        _cardViewTweens.AddActionInQueueOrInvokeImediatly( () => _rows[rowData.id].OpenCard( cardData ) );
    }

    public void AddStackOfCards(StackOfCardsData stackOfCardsData, RowData rowData)
    {
        foreach ( CardData card in stackOfCardsData.Cards )
            AddCardToRow( card, rowData );
    }

    public void RemoveStackOfCards(StackOfCardsData stackOfCardsData, RowData rowData)
    {
        _rows[rowData.id].RemoveStack(stackOfCardsData);
    }

    public void RemoveWinStack(StackOfCardsData stackOfCardsData, RowData rowData)
    {
        var cards = _rows[rowData.id].GetCardsByStack( stackOfCardsData );

        _cardViewTweens.TweenWinStack(cards, () => _rows[rowData.id].RemoveStack(stackOfCardsData) );
    }
}
