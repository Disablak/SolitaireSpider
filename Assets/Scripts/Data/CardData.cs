
public class CardData
{
    public CardColor color = CardColor.None;
    public CardType type = CardType.None;
    public bool isOpen = false;


    public CardData(CardType type, CardColor color)
    {
        this.type = type;
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

    One,
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