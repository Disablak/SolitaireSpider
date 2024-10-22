using UnityEngine;


public class CardViewFactory : MonoBehaviour
{
    [SerializeField] private CardView _cardViewPrefab;
    [SerializeField] private Transform _cardsContainer;

    private ObjectPool<CardView> _cardViewPool;


    public void Init()
    {
        _cardViewPool = new ObjectPool<CardView>(_cardViewPrefab.gameObject, _cardsContainer, 100);
    }

    public CardView GetCard()
    {
        return _cardViewPool.Pull();
    }

    public void PoolCard(CardView cardView)
    {
        _cardViewPool.Push(cardView.gameObject);
    }
}
