
public class CardData
{
    public int       id     = -1;
    public CardColor color  = CardColor.None;
    public CardType  type   = CardType.None;
    public bool      isOpen = false;


    public CardData(int id, CardType type, CardColor color)
    {
        this.id    = id;
        this.type  = type;
        this.color = color;
    }

    public void Open()
    {
        isOpen = true;
    }
}

public enum CardColor
{
    None,

    Black,
    Red,
    Green,
    Blue
}

public enum CardType
{
    None,

    Ace,

    Two,
    Three,
    Four,
    Five,
    Six,
    Seven,
    Eight,
    Nine,
    Ten,

    Jack,
    Queen,
    King
}