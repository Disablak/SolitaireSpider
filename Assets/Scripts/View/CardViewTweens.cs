using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;


public class CardViewTweens : MonoBehaviour
{
    [SerializeField] private Canvas _canvas;
    [SerializeField] private RectTransform _rtOtherCard;

    private Sequence _sequence;
    private List<Action> _onFinishCardsToRows = new List<Action>();

    public bool IsCardsTweening
        => _sequence != null && _sequence.IsActive() && _sequence.IsPlaying();

    private const float CARD_TWEEN_INTERVAL = 0.05f;
    private const float CARD_TWEEN_TIME = 0.5f;


    class TweenCardData
    {
        public CardView cardView;
        public RectTransform rtCard;
        public Vector2 from;
        public Vector2 to;
        public Action finishCallback;
    }

    public void TweenCardsToRows(Dictionary<CardView, RowView> dictionary)
    {
        List<TweenCardData> tweenCardData = new List<TweenCardData>();
        foreach ( KeyValuePair<CardView,RowView> pair in dictionary )
        {
            var rt  = pair.Key.GetComponent<RectTransform>();
            var fromPos = GetUIPositionRelativeToCanvas( _rtOtherCard,                      _canvas );
            var toPos   = GetUIPositionRelativeToCanvas( pair.Value.GetComponent<RectTransform>(), _canvas );

            tweenCardData.Add( new TweenCardData() {cardView = pair.Key, rtCard = rt, from = fromPos, to = toPos, finishCallback = () => pair.Value.AddCard( pair.Key )} );
        }

        _sequence = DOTween.Sequence();
        foreach ( TweenCardData data in tweenCardData )
        {
            _sequence.AppendInterval( CARD_TWEEN_INTERVAL )
                     .AppendCallback( () => data.cardView.ShowOrHide( true ) )
                     .AppendCallback( () => data.rtCard.DOAnchorPos( data.to, CARD_TWEEN_TIME ).From(data.from).SetEase( Ease.InOutSine ).OnComplete( () => data.finishCallback?.Invoke() ) );
        }
        _sequence.AppendInterval( CARD_TWEEN_TIME + 0.1f );
        _sequence.OnComplete( InvokeActions );
        _sequence.Play();
    }

    public void AddActionInQueueOrInvokeImediatly(Action onFinish)
    {
        if (IsCardsTweening)
            _onFinishCardsToRows.Add( onFinish );
        else
            onFinish?.Invoke();
    }

    private void InvokeActions()
    {
        _sequence = DOTween.Sequence();

        foreach ( Action action in _onFinishCardsToRows )
        {
            _sequence.AppendCallback( () => action?.Invoke() );
            _sequence.AppendInterval( CARD_TWEEN_INTERVAL );
        }
        _sequence.OnComplete( () => _onFinishCardsToRows.Clear() );
        _sequence.Play();
    }

    private Vector2 GetUIPositionRelativeToCanvas(RectTransform uiElement, Canvas canvas)
    {
        // Отримуємо екранні координати (у пікселях)
        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, uiElement.position);

        // Перетворюємо їх у локальні координати канваса
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, screenPoint, canvas.worldCamera, out Vector2 localPoint);

        return localPoint;
    }
}
