using System;
using System.Collections.Generic;
using System.Linq;


public class RowData
{
    public List<CardData> cards = new List<CardData>();
    public int id;

    public event Action<CardData, RowData>         OnCardRemoved  = delegate {};
    public event Action<StackOfCardsData, RowData> OnStackAdded   = delegate {};
    public event Action<StackOfCardsData, RowData> OnStackRemoved = delegate {};
    public event Action<CardData, RowData> OnCardOpened = delegate {};

    public RowData(int id)
    {
        this.id = id;
    }

    public CardData GetLastCard()
    {
        return cards.LastOrDefault();
    }

    public void AddCard(CardData card)
    {
        cards.Add(card);
        //TODO event on add card
        TryToRemoveValidSet();
    }

    public void AddStack(StackOfCardsData stack)
    {
        cards.AddRange(stack.Cards);
        OnStackAdded(stack, this);

        TryToRemoveValidSet();
    }

    public StackOfCardsData TakeStack(int cardId)
    {
        CardData card = cards.First(x => x.id == cardId);

        int indexOfCard = cards.IndexOf(card);
        List<CardData> stackOfCards = cards.Where( ( t, i ) => i >= indexOfCard ).ToList();

        return new StackOfCardsData(stackOfCards, id);
    }

    public void RemoveStack(StackOfCardsData stack)
    {
        cards = cards.Except( stack.Cards ).ToList();

        OnStackRemoved(stack, this);
    }

    public bool CanTakeStack(int startCardId)
    {
        CardData card = cards.FirstOrDefault(x => x.id == startCardId);
        if (card == null)
            return false;

        if (!card.isOpen)
            return false;

        int indexOfCard = cards.IndexOf(card);
        CardType checkCardType = card.type;
        for ( int i = indexOfCard; i < cards.Count; i++ )
        {
            CardData cardData = cards[i];

            if (cardData.type != checkCardType)
            {
                return false;
            }

            checkCardType--;
        }

        return true;
    }

    public bool CanAddStack(StackOfCardsData stack)
    {
        return CanAddCard( stack.FirstCard );
    }

    public bool CanAddCard(CardData card)
    {
        if (!card.isOpen)
        {
            return true;
        }

        CardData prevCard = GetLastCard();
        if (prevCard == null)
        {
            return true;
        }

        if (card.type == CardType.King)
        {
            return false;
        }

        CardType cardTypeNeed = card.type + 1;
        return prevCard.type == cardTypeNeed;
    }

    public void OpenLastCard()
    {
        CardData last = GetLastCard();
        if (last == null)
            return;

        last.Open();
        OnCardOpened(last, this);
    }

    public bool IsValidSet()
    {
        if (cards.Count < (int)CardType.King)
            return false;

        CardType checkType = CardType.Ace;
        for ( int i = cards.Count - 1; i >= 0; i-- )
        {
            CardData card = cards[i];

            if (!card.isOpen)
                return false;

            if (card.type == checkType)
            {
                if (checkType == CardType.King)
                    return true;

                checkType++;
            }
        }

        return false;
    }

    private void TryToRemoveValidSet()
    {
        if (!IsValidSet())
            return;

        for ( int i = cards.Count - 1; i >= 0; i-- ) // TODO refa
        {
            CardData card = cards[i];
            cards.Remove( card );

            OnCardRemoved(card, this);

            if (card.type == CardType.King)
            {
                return;
            }
        }
    }
}
