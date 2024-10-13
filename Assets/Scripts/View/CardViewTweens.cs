using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;


public class CardViewTweens : MonoBehaviour
{
    [SerializeField] private Canvas _canvas;
    [SerializeField] private RectTransform _rtOtherCard;

    private UniTask<UniTaskVoid> _currentTask;


    class TweenCardData
    {
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

            tweenCardData.Add( new TweenCardData() {rtCard = rt, from = fromPos, to = toPos, finishCallback = () => pair.Value.AddCard( pair.Key )} );
        }

        _currentTask = UniTask.RunOnThreadPool( () => AsyncTweenCards( tweenCardData ) );
    }

    private void Update()
    {
        Debug.LogError( _currentTask.GetAwaiter().IsCompleted );
    }

    private async UniTaskVoid AsyncTweenCards(List<TweenCardData> tweenCardData)
    {
        foreach ( TweenCardData data in tweenCardData )
        {
            await UniTask.Delay( TimeSpan.FromSeconds( 0.05f ));
            data.rtCard.DOAnchorPos( data.to, 0.3f ).From(data.from).SetEase( Ease.InOutSine ).OnComplete( () => data.finishCallback?.Invoke() );
        }
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
