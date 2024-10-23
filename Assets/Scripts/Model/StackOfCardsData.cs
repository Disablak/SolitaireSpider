using System.Collections.Generic;
using System.Linq;


public class StackOfCardsData
{
    private List<CardData> _cards;
    private int            _originRowId;

    public CardData       LastCard    => _cards.LastOrDefault();
    public CardData       FirstCard   => _cards.FirstOrDefault();
    public List<CardData> Cards       => _cards;
    public int            OriginRowId => _originRowId;


    public StackOfCardsData(List<CardData> cards, int originRowId)
    {
        _cards       = cards;
        _originRowId = originRowId;
    }
}