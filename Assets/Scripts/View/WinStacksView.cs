using System.Collections.Generic;
using UnityEngine;


public class WinStacksView : MonoBehaviour
{
    [SerializeField] private List<CardView> _winStackCards;

    private int _curWinStack = 0;


    private void Awake()
    {
        foreach (CardView card in _winStackCards)
            card.ShowOrHide( false );
    }

    public void ShowNextWinStack()
    {
        _winStackCards[_curWinStack++].ShowOrHide( true );
    }

    public Transform GetCurWinStackTransform()
    {
        return _winStackCards[_curWinStack].transform;
    }
}
