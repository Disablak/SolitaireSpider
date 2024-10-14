using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private SolitaireData _solitaireData;
    [SerializeField] private GameView _gameView;

    private GameModel _gameModel;

    private IEnumerator Start()
    {
        _gameModel                   =  new GameModel(_solitaireData);
        _gameModel.OnCardOpened      += OnCardOpened;
        _gameModel.OnCardsAddedToRows  += OnCardsAddedToRows;
        _gameModel.OnStackAdded      += OnStackAdded;
        _gameModel.OnWinStackRemoved += OnWinStackRemoved;
        _gameModel.OnStackRemoved    += OnStackRemoved;

        _gameView.Init( _gameModel );
        _gameView.AddRows( _gameModel.GetRowCount() );

        yield return null;

        _gameModel.StartSetup();
    }

    private void OnStackRemoved( StackOfCardsData arg1, RowData arg2 )
    {
        _gameView.RemoveStackOfCards(arg1, arg2);
    }

    private void OnWinStackRemoved( StackOfCardsData arg1, RowData arg2 )
    {
        _gameView.RemoveWinStack(arg1, arg2);
    }

    private void OnStackAdded( StackOfCardsData arg1, RowData arg2 )
    {
        _gameView.AddStackOfCards(arg1, arg2);
    }

    private void OnCardsAddedToRows( Dictionary<CardData, RowData> dictionary )
    {
        _gameView.AddCardsToRows( dictionary );
    }

    private void OnCardOpened( CardData card, RowData rowData )
    {
        _gameView.OpenCard( card, rowData );
    }
}
