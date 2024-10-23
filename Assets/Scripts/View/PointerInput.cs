using System;
using UnityEngine;


public class PointerInput : MonoBehaviour
{
    private StackOfCardsView _stickStackOfCardsView;

    public event Action OnMouseReleased = delegate {};


    private void Update()
    {
        if (_stickStackOfCardsView) ((RectTransform)_stickStackOfCardsView.transform).anchoredPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

        if (Input.GetMouseButtonUp(0))
        {
            OnMouseReleased();
            UnstickFromPointer();
        }
    }

    public void StickToPointer(StackOfCardsView stackOfCards)
    {
        _stickStackOfCardsView = stackOfCards;
    }

    public void UnstickFromPointer()
    {
        _stickStackOfCardsView = null;
    }
}