using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameView : MonoBehaviour
{
    [SerializeField] private RowUI _rowPrefab;
    [SerializeField] private CardView _cardPrefab;
    [SerializeField] private StackOfCardsView _stackOfCardsPrefab;
    [SerializeField] private Transform _rowContainer;
    [SerializeField] private PointerInput _pointerInput;


    private List<RowUI> _rows = new List<RowUI>();
    private GameModel _gameModel;
    public int _lastPointedRow = -1;
    private StackOfCardsView _currentStackOfCardsView;
    private StackOfCardsData _currentStackOfCardsData;

    public void Init(GameModel gameModel) // TODO VIEW NOT ABLE TO SEE MODEL
    {
        _gameModel = gameModel;

        _pointerInput.OnMouseReleased += OnPointerReleased;
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
        }

        if (_currentStackOfCardsView)
        {
            _currentStackOfCardsView.gameObject.SetActive( false );
            _currentStackOfCardsView = null;
        }
    }

    public void AddRows(int count)
    {
        for ( int i = 0; i < count; i++ )
        {
            RowUI row = Instantiate(_rowPrefab, _rowContainer);
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

    private void OnClickedCardInRow(CardView cardView, RowUI rowView)
    {
        if (!_gameModel.CanTakeStackOfCards( cardView.Id, rowView.Id ))
        {
            return;
        }

        StackOfCardsView stackOfCardsView = Instantiate( _stackOfCardsPrefab, transform );
        StackOfCardsData stackOfCardsData = _gameModel.TakeStackOfCards(cardView.Id, rowView.Id);

        stackOfCardsView.Init( stackOfCardsData );

        _pointerInput.StickToPointer(stackOfCardsView);
        _currentStackOfCardsView = stackOfCardsView;
        _currentStackOfCardsData = stackOfCardsData;
    }

    public void AddCardToRow(CardData card, int rowId)
    {
        var row      = _rows[rowId];
        var cardView = Instantiate( _cardPrefab, transform);
        cardView.Init(card);

        row.AddCard( cardView );
    }

    public void OpenCard(CardData cardData, RowData rowData)
    {
        _rows[rowData.id].OpenCard( cardData );
    }

    public void AddStackOfCards(StackOfCardsData stackOfCardsData, RowData rowData)
    {
        foreach ( CardData card in stackOfCardsData.Cards )
        {
            AddCardToRow( card, rowData.id );
        }
    }

    public void RemoveStackOfCards(StackOfCardsData stackOfCardsData, RowData rowData)
    {
        _rows[rowData.id].RemoveStack(stackOfCardsData);
    }
}
