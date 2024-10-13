using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private SolitaireData _solitaireData;
    [SerializeField] private GameView _gameView;

    private GameModel _gameModel;

    private void Awake()
    {
        _gameModel                   =  new GameModel(_solitaireData);
        _gameModel.OnCardOpened      += OnCardOpened;
        _gameModel.OnCardAddedToRow  += OnCardAddedToRow;
        _gameModel.OnStackAdded      += OnStackAdded;
        _gameModel.OnWinStackRemoved += OnWinStackRemoved;
        _gameModel.OnStackRemoved    += OnStackRemoved;

        //_gameModel.Initialize();

        _gameView.Init( _gameModel );
        _gameView.AddRows( _gameModel.GetRowCount() );

        _gameModel.StartSetup();
    }

    private void OnStackRemoved( StackOfCardsData arg1, RowData arg2 )
    {
        _gameView.RemoveStackOfCards(arg1, arg2);
    }

    private void OnWinStackRemoved( StackOfCardsData arg1, RowData arg2 )
    {
        _gameView.RemoveStackOfCards(arg1, arg2); // TODO with animation
    }

    private void OnStackAdded( StackOfCardsData arg1, RowData arg2 )
    {
        _gameView.AddStackOfCards(arg1, arg2);
    }

    private void OnCardAddedToRow( CardData card, RowData row )
    {
        _gameView.AddCardToRow( card, row.id );
    }

    private void OnCardOpened( CardData card, RowData rowData )
    {
        _gameView.OpenCard( card, rowData );
    }
}
