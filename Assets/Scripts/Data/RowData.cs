using System.Collections.Generic;
using System.Linq;


public class RowData
{
    public List<CardData> cards = new List<CardData>();


    public void AddCard(CardData card)
    {
        cards.Add(card);
    }

    public void OpenLastCard()
    {
        cards.Last().Open();
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
}
