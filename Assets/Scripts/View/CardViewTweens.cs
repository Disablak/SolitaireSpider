using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;


public class CardViewTweens : MonoBehaviour
{
    [SerializeField] private Canvas        _canvas;
    [SerializeField] private RectTransform _rtOtherCard;
    [SerializeField] private WinStacksView _winStacksView;

    [Space]
    [SerializeField] private float _cardTweenTime = 0.3f;
    [SerializeField] private float _cardTweenInterval = 0.01f;

    private CardViewFactory _cardFactory;
    private Sequence        _sequence;
    private List<Action>    _onFinishCardsToRows = new();

    public bool IsCardsTweening
        => _sequence != null && _sequence.IsActive() && _sequence.IsPlaying();


    private class TweenCardData
    {
        public CardView      cardView;
        public RectTransform rtCard;
        public Vector2       from;
        public Vector2       to;
        public Action        finishCallback;
    }


    public void Init(CardViewFactory cardFactory)
    {
        _cardFactory = cardFactory;
    }

    public void TweenWinStack(List<CardView> cards, Action finishCallback)
    {
        cards.Reverse();

        _sequence = DOTween.Sequence();
        _sequence.AppendInterval(0.1f);

        foreach (CardView card in cards)
            _sequence.AppendInterval(0.02f)
                     .AppendCallback(() =>
                     {
                         card.ShowOrHide(true);
                         card.transform.DOMove(_winStacksView.GetCurWinStackTransform().position, _cardTweenTime).SetEase(Ease.InOutSine);
                     });

        _sequence.AppendInterval(_cardTweenTime + 0.1f);
        _sequence.OnComplete(OnFinish);
        _sequence.Play();

        void OnFinish()
        {
            _winStacksView.ShowNextWinStack();
            finishCallback?.Invoke();
            InvokeActions();
        }
    }

    public void TweenCardsToRows(Dictionary<CardData, RowView> dictionary)
    {
        List<TweenCardData> tweenCardData = new();
        foreach (KeyValuePair<CardData, RowView> pair in dictionary)
        {
            CardView tweenCard = _cardFactory.GetCard();
            tweenCard.transform.SetParent(transform);
            tweenCard.Init(pair.Key);
            tweenCard.ShowOrHide(false);

            RectTransform rt      = tweenCard.GetComponent<RectTransform>();
            Vector2       fromPos = GetUIPositionRelativeToCanvas(_rtOtherCard,                                                      _canvas);
            Vector2       toPos   = GetUIPositionRelativeToCanvas(pair.Value.GetCardView(pair.Key.id).GetComponent<RectTransform>(), _canvas);

            tweenCardData.Add(new TweenCardData() {cardView = tweenCard, rtCard = rt, from = fromPos, to = toPos, finishCallback = OnFinish});

            void OnFinish()
            {
                _cardFactory.PoolCard(tweenCard);
                tweenCard.ShowOrHide(false);
                pair.Value.ShowOrHideCardSprite(pair.Key.id, true);
            }
        }

        _sequence = DOTween.Sequence();
        foreach (TweenCardData data in tweenCardData)
            _sequence.AppendInterval(_cardTweenInterval)
                     .AppendCallback(() => data.cardView.ShowOrHide(true))
                     .AppendCallback(() => data.rtCard.DOAnchorPos(data.to, _cardTweenTime).From(data.from).SetEase(Ease.InOutSine).OnComplete(() => data.finishCallback?.Invoke()));

        _sequence.AppendInterval(_cardTweenTime + 0.1f);
        _sequence.OnComplete(InvokeActions);
        _sequence.Play();
    }

    public void AddActionInQueueOrInvokeImediatly(Action onFinish)
    {
        if (IsCardsTweening)
            _onFinishCardsToRows.Add(onFinish);
        else
            onFinish?.Invoke();
    }

    private void InvokeActions()
    {
        _sequence = DOTween.Sequence();

        foreach (Action action in _onFinishCardsToRows)
        {
            _sequence.AppendCallback(() => action?.Invoke());
            _sequence.AppendInterval(_cardTweenInterval);
        }

        _sequence.OnComplete(() => _onFinishCardsToRows.Clear());
        _sequence.Play();
    }

    private Vector2 GetUIPositionRelativeToCanvas(RectTransform uiElement, Canvas canvas)
    {
        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, uiElement.position);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, screenPoint, canvas.worldCamera, out Vector2 localPoint);

        return localPoint;
    }
}