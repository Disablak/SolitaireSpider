using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GamePresenter : MonoBehaviour
{
    [SerializeField] private GameData _gameData;
    [SerializeField] private GameView _gameView;

    private GameModel _gameModel;


    private IEnumerator Start()
    {
        _gameModel = new GameModel(_gameData);
        _gameModel.OnCardOpened += OnCardOpened;
        _gameModel.OnCardsAddedToRows += OnCardsAddedToRows;
        _gameModel.OnStackAdded += OnStackAdded;
        _gameModel.OnWinStackRemoved += OnWinStackRemoved;
        _gameModel.OnStackRemoved += OnStackRemoved;
        _gameModel.OnGameOver += OnGameOver;

        _gameView.AddRows( _gameModel.GetRowCount() );

        yield return null;

        _gameModel.StartSetup();
    }

    private void OnGameOver( bool win )
    {
        Debug.Log( $"Game Over ({win})" );
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

    public bool CanAddNewCards()
    {
        return _gameModel.CanAddCards();
    }

    public bool CanAddStackOfCards(StackOfCardsData stack, int rowId)
    {
        return _gameModel.CanAddStackOfCards(stack, rowId);
    }

    public bool CanTakeStackOfCards(int cardId, int rowId)
    {
        return _gameModel.CanTakeStackOfCards(cardId, rowId);
    }

    public StackOfCardsData GetStackOfCards(int cardId, int rowId)
    {
        return _gameModel.GetStackOfCards(cardId, rowId);
    }

    public void RemoveStackOfCards(StackOfCardsData stack, int rowId)
    {
        _gameModel.RemoveStackOfCards(stack, rowId);
    }

    public void AddStackOfCards(StackOfCardsData stack, int rowId)
    {
        _gameModel.AddStackOfCards(stack, rowId);
    }

    public void AddNewCardsAndOpenLast()
    {
        _gameModel.AddCardsFromDeckToRows();
        _gameModel.OpenLastCardInRows();
    }

    public void MoveStackOfCards(StackOfCardsData stack, int rowIdFrom, int rowIdTo)
    {
        RemoveStackOfCards(stack, rowIdFrom);
        AddStackOfCards(stack, rowIdTo);
    }
}
