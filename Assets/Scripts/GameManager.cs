using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private SolitaireData _solitaireData;

    private GameModel _gameModel;

    private void Awake()
    {
        _gameModel = new GameModel();
    }
}
